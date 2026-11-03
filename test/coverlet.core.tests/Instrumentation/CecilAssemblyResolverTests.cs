// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using Coverlet.Core.Abstractions;
using Coverlet.Core.Instrumentation;
using Mono.Cecil;
using Moq;
using Xunit;

namespace Coverlet.Core.Tests.Instrumentation
{
  public class CecilAssemblyResolverTests
  {
    [Theory]
    [InlineData("StringComparison")]
    [InlineData("MidpointRounding")]
    public void RuntimeNetstandardFacade_UsesConfiguredResolverForForwardedEnums(string enumName)
    {
      string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(directory);
      try
      {
        using ModuleDefinition primitiveTypes = ModuleDefinition.CreateModule("PrimitiveTypes", ModuleKind.Dll);
        using var resolver = new NetstandardAwareAssemblyResolver(null, Mock.Of<ILogger>());
        AssemblyDefinition facade = resolver.Resolve(AssemblyNameReference.Parse(
          "netstandard, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"));
        ExportedType forwardedEnum = facade.MainModule.ExportedTypes.Single(type => type.FullName == "System." + enumName);
        var frameworkReference = (AssemblyNameReference)forwardedEnum.Scope;
        string frameworkPath = Path.Combine(directory, frameworkReference.Name + ".dll");

        // A private framework search path stands in for the resolver's runtime/reference
        // assembly fallbacks. Cecil's default resolver cannot see this directory.
        using (AssemblyDefinition framework = AssemblyDefinition.CreateAssembly(
          new AssemblyNameDefinition(frameworkReference.Name, frameworkReference.Version),
          frameworkReference.Name, ModuleKind.Dll))
        {
          framework.MainModule.Types.Add(new TypeDefinition("System", "Int32", TypeAttributes.Public | TypeAttributes.Sealed,
            new TypeReference("System", "ValueType", framework.MainModule, framework.MainModule.TypeSystem.CoreLibrary)));
          var enumType = new TypeDefinition("System", enumName,
            TypeAttributes.Public | TypeAttributes.Sealed,
            new TypeReference("System", "Enum", framework.MainModule, framework.MainModule.TypeSystem.CoreLibrary));
          enumType.Fields.Add(new FieldDefinition("value__", FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName,
            primitiveTypes.TypeSystem.Int32));
          framework.MainModule.Types.Add(enumType);
          framework.Write(frameworkPath);
        }

        resolver.AddSearchDirectory(directory);

        TypeDefinition resolvedEnum = forwardedEnum.Resolve();

        Assert.Equal(frameworkPath, resolvedEnum.Module.FileName);
        Assert.True(resolvedEnum.IsEnum);

        // Writing optional enum constants is the metadata path that failed on .NET Framework.
        using AssemblyDefinition consumer = AssemblyDefinition.CreateAssembly(
          new AssemblyNameDefinition("EnumConsumer", new Version(1, 0)), "EnumConsumer",
          new ModuleParameters { Kind = ModuleKind.Dll, AssemblyResolver = resolver });
        var consumerType = new TypeDefinition("Example", "Consumer", TypeAttributes.Public, consumer.MainModule.TypeSystem.Object);
        consumer.MainModule.Types.Add(consumerType);
        AssemblyNameReference facadeReference = AssemblyNameReference.Parse(facade.Name.FullName);
        consumer.MainModule.AssemblyReferences.Add(facadeReference);
        var method = new MethodDefinition("Choose", MethodAttributes.Public | MethodAttributes.Static, consumer.MainModule.TypeSystem.Int32);
        method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Optional | ParameterAttributes.HasDefault,
          new TypeReference("System", enumName, consumer.MainModule, facadeReference)) { Constant = 0 });
        method.Body.GetILProcessor().Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
        method.Body.GetILProcessor().Emit(Mono.Cecil.Cil.OpCodes.Ret);
        consumerType.Methods.Add(method);
        using var output = new MemoryStream();
        consumer.Write(output);
        Assert.NotEqual(0, output.Length);
      }
      finally
      {
        Directory.Delete(directory, recursive: true);
      }
    }
  }
}
