using RazorEngine.Templating;
using System;

namespace Omnae.Libs
{
    public class MyTemplateManager : ITemplateManager
    {
        private readonly string baseTemplatePath;
        public MyTemplateManager(string basePath)
        {
            baseTemplatePath = basePath;
        }

        public ITemplateSource Resolve(ITemplateKey key)
        {
            string template = key.Name;
            //    string path = Path.Combine(baseTemplatePath, string.Format("{0}{1}", template, ".html"));
            //    string content = File.ReadAllText(path);
            //    return new LoadedTemplateSource(content, path);
            return new LoadedTemplateSource(template, null);
        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotImplementedException("dynamic templates are not supported!");
        }
    }
}
