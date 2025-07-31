using System;
using System.Globalization;
using System.Resources;

namespace ChatbotWPF
{
    /// <summary>
    /// Interface for managing translation within an application.
    /// </summary>
    public interface ITranslationManager
    {
        /// <summary>
        /// Gets a localized resource based on the specified resource ID and optional parameters.
        /// </summary>
        /// <param name="resourceId">The identifier of the resource</param>
        /// <param name="param">Optional parameters to be formatted into the resource string</param>
        /// <returns>The localized resource string.</returns>
        string GetResource(string resourceId, params object[] param);

        /// <summary>
        /// Set the current culture
        /// </summary>
        /// <param name="culture"></param>
        public void SetCulture(CultureInfo culture);

        /// <summary>
        /// Get the current culture
        /// </summary>
        /// <returns>CultureInfo</returns>
        public CultureInfo GetCulture();
    }

    /// <summary>
    /// An implementation of ITranslationManager using ResourceManager.
    /// </summary>
    public class TranslationManager : ITranslationManager
    {
        private const string ResourceBaseName = "ChatbotWPF.Resources";

        private readonly ResourceManager _resourceManager;
        private CultureInfo _culture;

        /// <summary>
        /// Create the manager for a given resource base name and assembly.
        /// </summary>
        /// <param name="type">Any type from the assembly where the resource files are embedded.</param>
        /// <param name="defaultCulture">Default culture if none is set explicitly.</param>
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

