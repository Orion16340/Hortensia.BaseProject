using System;

namespace Hortensia.Core
{
    public static class ServiceLocator
    {
        private static IServiceProvider m_provider;

        public static IServiceProvider Provider
        {
            get => m_provider ?? throw new NullReferenceException($"You must set {nameof(m_provider)} before using {nameof(ServiceLocator)}");
            set => m_provider = (m_provider == default) ? value : m_provider;
        }
    }
}
