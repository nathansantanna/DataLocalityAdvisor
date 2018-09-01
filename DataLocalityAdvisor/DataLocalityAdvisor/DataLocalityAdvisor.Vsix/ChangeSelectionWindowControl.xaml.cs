namespace DataLocalityAdvisor.Vsix
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ChangeSelectionWindowControl.
    /// </summary>
    public partial class ChangeSelectionWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSelectionWindowControl"/> class.
        /// </summary>
        public ChangeSelectionWindowControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            List<CheckBox> Changes = new List<CheckBox>();
            for (int i = 0; i < 10; i++)
            {
                CheckBox c = new CheckBox();
                c.Content = "PossibleChange" + i;
                Changes.Add(c);
                ChangeList.Items.Add(Changes[i]);
            }

        }
    }
}