export default {
  start: () => {
    const script = document.createElement('script');
    script.src = 'https://context7.com/widget.js';
    script.setAttribute('data-library', '/codebeltnet/coverlet');
    script.setAttribute('data-color', '#059669');
    script.setAttribute('data-position', 'bottom-right');
    script.setAttribute('data-placeholder', 'Ask about Codebelt.Coverlet.MTP...');
    script.async = true;
    document.body.appendChild(script);
  }
};
