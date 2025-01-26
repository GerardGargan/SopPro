using System.Collections.Concurrent;
using Backend.Service.Interface;
using HandlebarsDotNet;

namespace Backend.Serivce.Implementation
{
    public class TemplateService : ITemplateService
    {

        private readonly string _templateFolder;
        private readonly ConcurrentDictionary<string, HandlebarsTemplate<object, object>> _compiledTemplates;

        public TemplateService(string templateFolder)
        {
            _templateFolder = templateFolder;
            _compiledTemplates = new ConcurrentDictionary<string, HandlebarsTemplate<object, object>>();
        }

        public async Task<string> RenderTemplateAsync(string templateName, object model)
        {
            var template = await GetOrCompileTemplateAsync(templateName);
            return template(model);
        }

        private async Task<HandlebarsTemplate<object, object>> GetOrCompileTemplateAsync(string templateName)
        {
            if (_compiledTemplates.TryGetValue(templateName, out var compiledTemplate))
            {
                return compiledTemplate;
            }

            var templatePath = Path.Combine(_templateFolder, $"{templateName}.hbs");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template '{templateName}' not found at '{templatePath}'");
            }

            var templateContent = await File.ReadAllTextAsync(templatePath);
            compiledTemplate = Handlebars.Compile(templateContent);

            _compiledTemplates.TryAdd(templateName, compiledTemplate);
            return compiledTemplate;
        }
    }
}