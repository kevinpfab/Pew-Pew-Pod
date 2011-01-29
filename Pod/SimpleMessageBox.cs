using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.GamerServices;

namespace Pod
{
    class SimpleMessageBox
    {
        private static int? dialogResult = null;
        public static bool Showing { get; set; }

        public static int? ShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon)
        {
            // don't do anything if the guide is visible - one issue this handles is showing dialogs in quick
            // succession, we have to wait for the guide to go away before the next dialog can display
            if (Guide.IsVisible) return null;

            // if we have a result then we're all done and we want to return it
            if (dialogResult != null)
            {
                // preserve the result
                int? saveResult = dialogResult;

                // reset everything for the next message box
                dialogResult = null;
                Showing = false;

                // return the result
                return saveResult;
            }

            // return nothing if the message box is still being displayed
            if (Showing) return null;

            // otherwise show it
            Showing = true;
            Guide.BeginShowMessageBox(title, text, buttons, focusButton, icon, MessageBoxEnd, null);
            return null;
        }

        private static void MessageBoxEnd(IAsyncResult result)
        {
            dialogResult = Guide.EndShowMessageBox(result);

            // if no button was pressed then we want the result to be -1
            if (dialogResult == null)
                dialogResult = -1;
        }
    }
}
