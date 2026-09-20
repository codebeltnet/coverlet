// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Reporters;
using Coverlet.Core.Samples.Tests;
using Moq;
using Xunit;

namespace Coverlet.Core.Tests
{
  public partial class CoverageTests
  {
    [Fact]
    public async Task ReporterOutputsRemainSemanticallyEquivalentToCollectedCoverage()
    {
      CoverageResult coverageResult = await CollectReporterEquivalenceCoverageResultAsync();

      Modules jsonModules = JsonSerializer.Deserialize<Modules>(
          new JsonReporter().Report(coverageResult, new Mock<ISourceRootTranslator>().Object),
          new JsonSerializerOptions { IncludeFields = true });

      Assert.NotNull(jsonModules);
      AssertEquivalentModules(coverageResult.Modules, jsonModules);
      AssertCoverageDetailsEqual(CoverageSummary.CalculateLineCoverage(coverageResult.Modules), CoverageSummary.CalculateLineCoverage(jsonModules));
      AssertCoverageDetailsEqual(CoverageSummary.CalculateBranchCoverage(coverageResult.Modules), CoverageSummary.CalculateBranchCoverage(jsonModules));
      AssertCoverageDetailsEqual(CoverageSummary.CalculateMethodCoverage(coverageResult.Modules), CoverageSummary.CalculateMethodCoverage(jsonModules));

      AssertOpenCoverReportMatchesCoverageResult(
          coverageResult,
          new OpenCoverReporter().Report(coverageResult, new Mock<ISourceRootTranslator>().Object));
      AssertCoberturaReportMatchesCoverageResult(
          coverageResult,
          new CoberturaReporter().Report(coverageResult, new Mock<ISourceRootTranslator>().Object));
    }

    private static async Task<CoverageResult> CollectReporterEquivalenceCoverageResultAsync()
    {
      string prepareResultPath = Path.GetTempFileName();

      try
      {
        await TestInstrumentationHelper.Run<RelationalPatternBranch>(
            instance =>
            {
              instance.IsLowerInSimpleIf('a');
              return Task.CompletedTask;
            },
            persistPrepareResultToFile: prepareResultPath);

        return TestInstrumentationHelper.GetCoverageResult(prepareResultPath);
      }
      finally
      {
        if (File.Exists(prepareResultPath))
        {
          File.Delete(prepareResultPath);
        }
      }
    }

    private static void AssertEquivalentModules(Modules expected, Modules actual)
    {
      Assert.Equal(expected.Keys.OrderBy(k => k, StringComparer.Ordinal), actual.Keys.OrderBy(k => k, StringComparer.Ordinal));

      foreach ((string moduleName, Documents expectedDocuments) in expected)
      {
        Documents actualDocuments = actual[moduleName];
        Assert.Equal(expectedDocuments.Keys.OrderBy(k => k, StringComparer.Ordinal), actualDocuments.Keys.OrderBy(k => k, StringComparer.Ordinal));

        foreach ((string documentName, Classes expectedClasses) in expectedDocuments)
        {
          Classes actualClasses = actualDocuments[documentName];
          Assert.Equal(expectedClasses.Keys.OrderBy(k => k, StringComparer.Ordinal), actualClasses.Keys.OrderBy(k => k, StringComparer.Ordinal));

          foreach ((string className, Methods expectedMethods) in expectedClasses)
          {
            Methods actualMethods = actualClasses[className];
            Assert.Equal(expectedMethods.Keys.OrderBy(k => k, StringComparer.Ordinal), actualMethods.Keys.OrderBy(k => k, StringComparer.Ordinal));

            foreach ((string methodName, Method expectedMethod) in expectedMethods)
            {
              Method actualMethod = actualMethods[methodName];
              Assert.Equal(expectedMethod.Lines.Count, actualMethod.Lines.Count);
              foreach ((int lineNumber, int hits) in expectedMethod.Lines)
              {
                Assert.True(actualMethod.Lines.TryGetValue(lineNumber, out int actualHits));
                Assert.Equal(hits, actualHits);
              }
              Assert.Equal(OrderBranches(expectedMethod.Branches), OrderBranches(actualMethod.Branches));
            }
          }
        }
      }
    }

    private static void AssertOpenCoverReportMatchesCoverageResult(CoverageResult coverageResult, string report)
    {
      var document = XDocument.Load(new StringReader(report));
      XElement coverageSession = document.Root;
      Assert.NotNull(coverageSession);

      CoverageDetails totalLineCoverage = CoverageSummary.CalculateLineCoverage(coverageResult.Modules);
      CoverageDetails totalBranchCoverage = CoverageSummary.CalculateBranchCoverage(coverageResult.Modules);
      CoverageDetails totalMethodCoverage = CoverageSummary.CalculateMethodCoverage(coverageResult.Modules);
      AssertOpenCoverSummary(
          coverageSession.Element("Summary"),
          totalLineCoverage,
          totalBranchCoverage,
          CountClasses(coverageResult.Modules),
          CountVisitedClasses(coverageResult.Modules),
          (int)totalMethodCoverage.Total,
          (int)totalMethodCoverage.Covered);

      XElement xmlModules = coverageSession.Element("Modules");
      Assert.NotNull(xmlModules);
      Assert.Equal(coverageResult.Modules.Count, xmlModules.Elements("Module").Count());

      foreach ((string modulePath, Documents documents) in coverageResult.Modules)
      {
        XElement xmlModule = xmlModules
            .Elements("Module")
            .Single(m => string.Equals((string)m.Element("ModulePath"), modulePath, StringComparison.Ordinal));

        Assert.Equal(Path.GetFileNameWithoutExtension(modulePath), (string)xmlModule.Element("ModuleName"));

        var fileIds = xmlModule
            .Element("Files")
            .Elements("File")
            .ToDictionary(
                element => (string)element.Attribute("fullPath"),
                element => (string)element.Attribute("uid"),
                StringComparer.Ordinal);

        Assert.Equal(documents.Count, fileIds.Count);

        foreach ((string documentPath, Classes classes) in documents)
        {
          string fileId = fileIds[documentPath];

          foreach ((string className, Methods methods) in classes)
          {
            XElement xmlClass = xmlModule
                .Element("Classes")
                .Elements("Class")
                .Single(c => string.Equals((string)c.Element("FullName"), className, StringComparison.Ordinal));

            CoverageDetails classLineCoverage = CoverageSummary.CalculateLineCoverage(methods);
            CoverageDetails classBranchCoverage = CoverageSummary.CalculateBranchCoverage(methods);
            CoverageDetails classMethodCoverage = CoverageSummary.CalculateMethodCoverage(methods);
            AssertOpenCoverSummary(
                xmlClass.Element("Summary"),
                classLineCoverage,
                classBranchCoverage,
                1,
                classLineCoverage.Covered > 0 ? 1 : 0,
                (int)classMethodCoverage.Total,
                (int)classMethodCoverage.Covered);

            foreach ((string methodName, Method method) in methods)
            {
              if (method.Lines.Count == 0)
              {
                continue;
              }

              XElement xmlMethod = xmlClass
                  .Element("Methods")
                  .Elements("Method")
                  .Single(m => string.Equals((string)m.Element("Name"), methodName, StringComparison.Ordinal));

              CoverageDetails methodLineCoverage = CoverageSummary.CalculateLineCoverage(method.Lines);
              CoverageDetails methodBranchCoverage = CoverageSummary.CalculateBranchCoverage(method.Branches);
              AssertOpenCoverSummary(
                  xmlMethod.Element("Summary"),
                  methodLineCoverage,
                  methodBranchCoverage,
                  0,
                  0,
                  1,
                  methodLineCoverage.Covered > 0 ? 1 : 0);

              XElement[] xmlSequencePoints = xmlMethod.Element("SequencePoints").Elements("SequencePoint").ToArray();
              Assert.Equal(method.Lines.Count, xmlSequencePoints.Length);

              foreach ((int lineNumber, int hits) in method.Lines)
              {
                XElement xmlLine = xmlSequencePoints.Single(point => GetIntAttribute(point, "sl") == lineNumber);
                BranchInfo[] branchesForLine = [.. method.Branches.Where(branch => branch.Line == lineNumber)];
                CoverageDetails branchCoverageForLine = CoverageSummary.CalculateBranchCoverage(branchesForLine);

                Assert.Equal(hits, GetIntAttribute(xmlLine, "vc"));
                Assert.Equal(branchCoverageForLine.Total, GetIntAttribute(xmlLine, "bec"));
                Assert.Equal((int)branchCoverageForLine.Covered, GetIntAttribute(xmlLine, "bev"));
                Assert.Equal(fileId, (string)xmlLine.Attribute("fileid"));
              }

              (int line, uint ordinal, int path, int offset, int endOffset, int hits)[] expectedBranches = OrderBranches(method.Branches);
              (int line, uint ordinal, int path, int offset, int endOffset, int hits)[] actualBranches = xmlMethod
                  .Element("BranchPoints")
                  .Elements("BranchPoint")
                  .Select(point => (
                      line: GetIntAttribute(point, "sl"),
                      ordinal: GetUIntAttribute(point, "ordinal"),
                      path: GetIntAttribute(point, "path"),
                      offset: GetIntAttribute(point, "offset"),
                      endOffset: GetIntAttribute(point, "offsetend"),
                      hits: GetIntAttribute(point, "vc")))
                  .OrderBy(branch => branch.line)
                  .ThenBy(branch => branch.offset)
                  .ThenBy(branch => branch.endOffset)
                  .ThenBy(branch => branch.path)
                  .ThenBy(branch => branch.ordinal)
                  .ThenBy(branch => branch.hits)
                  .ToArray();

              Assert.Equal(expectedBranches, actualBranches);
            }
          }
        }
      }
    }

    private static void AssertCoberturaReportMatchesCoverageResult(CoverageResult coverageResult, string report)
    {
      var document = XDocument.Load(new StringReader(report));
      XElement coverage = document.Root;
      Assert.NotNull(coverage);

      CoverageDetails totalLineCoverage = CoverageSummary.CalculateLineCoverage(coverageResult.Modules);
      CoverageDetails totalBranchCoverage = CoverageSummary.CalculateBranchCoverage(coverageResult.Modules);
      AssertRateAttribute(coverage, "line-rate", totalLineCoverage.Percent / 100D);
      AssertRateAttribute(coverage, "branch-rate", totalBranchCoverage.Percent / 100D);
      Assert.Equal(totalLineCoverage.Covered.ToString(CultureInfo.InvariantCulture), (string)coverage.Attribute("lines-covered"));
      Assert.Equal(totalLineCoverage.Total.ToString(CultureInfo.InvariantCulture), (string)coverage.Attribute("lines-valid"));
      Assert.Equal(totalBranchCoverage.Covered.ToString(CultureInfo.InvariantCulture), (string)coverage.Attribute("branches-covered"));
      Assert.Equal(totalBranchCoverage.Total.ToString(CultureInfo.InvariantCulture), (string)coverage.Attribute("branches-valid"));

      XElement packages = coverage.Element("packages");
      Assert.NotNull(packages);
      Assert.Equal(coverageResult.Modules.Count, packages.Elements("package").Count());

      foreach ((string modulePath, Documents documents) in coverageResult.Modules)
      {
        XElement package = packages
            .Elements("package")
            .Single(p => string.Equals((string)p.Attribute("name"), Path.GetFileNameWithoutExtension(modulePath), StringComparison.Ordinal));

        AssertRateAttribute(package, "line-rate", CoverageSummary.CalculateLineCoverage(documents).Percent / 100D);
        AssertRateAttribute(package, "branch-rate", CoverageSummary.CalculateBranchCoverage(documents).Percent / 100D);

        foreach ((string documentPath, Classes classes) in documents)
        {
          foreach ((string className, Methods methods) in classes)
          {
            XElement xmlClass = package
                .Element("classes")
                .Elements("class")
                .Single(c => string.Equals((string)c.Attribute("name"), className, StringComparison.Ordinal));

            Assert.Equal(Path.GetFileName(documentPath), Path.GetFileName((string)xmlClass.Attribute("filename")));
            AssertRateAttribute(xmlClass, "line-rate", CoverageSummary.CalculateLineCoverage(methods).Percent / 100D);
            AssertRateAttribute(xmlClass, "branch-rate", CoverageSummary.CalculateBranchCoverage(methods).Percent / 100D);

            foreach ((string methodName, Method method) in methods)
            {
              if (method.Lines.Count == 0)
              {
                continue;
              }

              string expectedMethodName = methodName.Split(':').Last().Split('(').First();
              string expectedMethodSignature = "(" + methodName.Split(':').Last().Split('(').Last();
              XElement xmlMethod = xmlClass
                  .Element("methods")
                  .Elements("method")
                  .Single(m =>
                      string.Equals((string)m.Attribute("name"), expectedMethodName, StringComparison.Ordinal) &&
                      string.Equals((string)m.Attribute("signature"), expectedMethodSignature, StringComparison.Ordinal));

              AssertRateAttribute(xmlMethod, "line-rate", CoverageSummary.CalculateLineCoverage(method.Lines).Percent / 100D);
              AssertRateAttribute(xmlMethod, "branch-rate", CoverageSummary.CalculateBranchCoverage(method.Branches).Percent / 100D);

              XElement[] xmlLines = xmlMethod.Element("lines").Elements("line").ToArray();
              Assert.Equal(method.Lines.Count, xmlLines.Length);

              foreach ((int lineNumber, int hits) in method.Lines)
              {
                XElement xmlLine = xmlLines.Single(line => GetIntAttribute(line, "number") == lineNumber);
                BranchInfo[] branchesForLine = [.. method.Branches.Where(branch => branch.Line == lineNumber)];
                CoverageDetails branchCoverageForLine = CoverageSummary.CalculateBranchCoverage(branchesForLine);

                Assert.Equal(hits, GetIntAttribute(xmlLine, "hits"));
                Assert.Equal((branchesForLine.Length > 0).ToString(), (string)xmlLine.Attribute("branch"));

                if (branchesForLine.Length > 0)
                {
                  Assert.Equal(
                      $"{branchCoverageForLine.Percent.ToString(CultureInfo.InvariantCulture)}% ({branchCoverageForLine.Covered.ToString(CultureInfo.InvariantCulture)}/{branchCoverageForLine.Total.ToString(CultureInfo.InvariantCulture)})",
                      (string)xmlLine.Attribute("condition-coverage"));
                }
                else
                {
                  Assert.Null(xmlLine.Attribute("condition-coverage"));
                }
              }
            }
          }
        }
      }
    }

    private static void AssertOpenCoverSummary(
        XElement summary,
        CoverageDetails lineCoverage,
        CoverageDetails branchCoverage,
        int expectedClasses,
        int visitedClasses,
        int expectedMethods,
        int visitedMethods)
    {
      Assert.NotNull(summary);
      Assert.Equal(lineCoverage.Total.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("numSequencePoints"));
      Assert.Equal(lineCoverage.Covered.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("visitedSequencePoints"));
      Assert.Equal(branchCoverage.Total.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("numBranchPoints"));
      Assert.Equal(branchCoverage.Covered.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("visitedBranchPoints"));
      Assert.Equal(lineCoverage.Percent.ToString("G", CultureInfo.InvariantCulture), (string)summary.Attribute("sequenceCoverage"));
      Assert.Equal(branchCoverage.Percent.ToString("G", CultureInfo.InvariantCulture), (string)summary.Attribute("branchCoverage"));
      Assert.Equal(expectedClasses.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("numClasses"));
      Assert.Equal(visitedClasses.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("visitedClasses"));
      Assert.Equal(expectedMethods.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("numMethods"));
      Assert.Equal(visitedMethods.ToString(CultureInfo.InvariantCulture), (string)summary.Attribute("visitedMethods"));
    }

    private static void AssertRateAttribute(XElement element, string attributeName, double expectedValue)
    {
      Assert.Equal(expectedValue.ToString(CultureInfo.InvariantCulture), (string)element.Attribute(attributeName));
    }

    private static void AssertCoverageDetailsEqual(CoverageDetails expected, CoverageDetails actual)
    {
      Assert.Equal(expected.Covered, actual.Covered);
      Assert.Equal(expected.Total, actual.Total);
      Assert.Equal(expected.Percent, actual.Percent);
      Assert.Equal(expected.AverageModulePercent, actual.AverageModulePercent);
    }

    private static int CountClasses(Modules modules)
    {
      return modules.Values.Sum(documents => documents.Values.Sum(classes => classes.Count));
    }

    private static int CountVisitedClasses(Modules modules)
    {
      return modules.Values
          .SelectMany(documents => documents.Values)
          .Count(methods => CoverageSummary.CalculateLineCoverage(methods).Covered > 0);
    }

    private static (int line, uint ordinal, int path, int offset, int endOffset, int hits)[] OrderBranches(Branches branches)
    {
      return [.. branches
          .Select(branch => (branch.Line, branch.Ordinal, branch.Path, branch.Offset, branch.EndOffset, branch.Hits))
          .OrderBy(branch => branch.Line)
          .ThenBy(branch => branch.Offset)
          .ThenBy(branch => branch.EndOffset)
          .ThenBy(branch => branch.Path)
          .ThenBy(branch => branch.Ordinal)
          .ThenBy(branch => branch.Hits)];
    }

    private static int GetIntAttribute(XElement element, string attributeName)
    {
      return int.Parse((string)element.Attribute(attributeName), CultureInfo.InvariantCulture);
    }

    private static uint GetUIntAttribute(XElement element, string attributeName)
    {
      return uint.Parse((string)element.Attribute(attributeName), CultureInfo.InvariantCulture);
    }
  }
}
