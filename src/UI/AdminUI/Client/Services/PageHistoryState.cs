using System.Collections.Generic;

namespace Siccar.UI.Admin.Services
{
    public class PageHistoryState
    {
        private readonly Stack<string> previousPages;

        public PageHistoryState()
        {
            previousPages = new Stack<string>();
        }
        public void AddPageToHistory(string pageName)
        {
            previousPages.Push(pageName);
        }

        public string GetGoBackPage()
        {
            if (previousPages.Count > 1)
            {
                // You add a page on initialization, so you need to return the 2nd from the last
                previousPages.Pop();
                if (previousPages.Count > 1) return previousPages.Pop();
                return "/";
            }

            // Can't go back because you didn't navigate enough
            return "/";
        }

        public bool CanGoBack()
        {
            return previousPages.Count > 1;
        }
    }
}
