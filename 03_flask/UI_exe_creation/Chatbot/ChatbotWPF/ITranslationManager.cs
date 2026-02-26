using System;
using System.Globalization;
using System.Resources;

namespace ChatbotWPF
{ 
    public interface ITranslationManager
    {
        string GetResource(string resourceId, params object[] param);

        public void SetCulture(CultureInfo culture);

        public CultureInfo GetCulture();
    }

    public class TranslationManager : ITranslationManager
    {
        private const string ResourceBaseName = "ChatbotWPF.Resources";

        private readonly ResourceManager _resourceManager;
        private CultureInfo _culture;

        public TranslationManager(Type type, CultureInfo? defaultCulture = null)
        {
            // You can adapt the string below to match your .resx namespace
            _resourceManager = new ResourceManager(ResourceBaseName, type.Assembly);
            _culture = defaultCulture ?? CultureInfo.InvariantCulture;
        }

        public string GetResource(string resourceId, params object[] param)
        {
            string? resourceValue = _resourceManager.GetString(resourceId, _culture);
            if (resourceValue == null)
            {
                resourceValue = $"[{resourceId}]";
            }
            return string.Format(resourceValue, param);
        }

        public void SetCulture(CultureInfo culture)
        {
            _culture = culture;
        }

        public CultureInfo GetCulture()
        {
            return _culture;
        }
    }
}

