namespace ReSharper.Exceptional.Utilities
{
    public static class NamespaceChecker
    {
        public static bool HasNamespaceMatch(string qualifiedNamespaceName, string namespaceToMatch)
        {
            var names = qualifiedNamespaceName.Split('.');

            foreach (var name in names)
            {
                if (name.EqualsWildcard(namespaceToMatch, true))
                {
                    return true;
                }
            }

            return false;
        }
    }
}