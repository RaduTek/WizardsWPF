using System;
using System.Windows;
using System.Windows.Controls;

namespace WizardsWPF
{
    /// <summary>
    /// Event argument class for WizardHelper.SelectedPageChanged event
    /// </summary>
    internal class SelectedPageChangedEventArgs : EventArgs
    {
        public int PreviousIndex { get; private set; }
        public int CurrentIndex { get; private set; }

        public SelectedPageChangedEventArgs(int previousIndex, int currentIndex)
        {
            PreviousIndex = previousIndex;
            CurrentIndex = currentIndex;
        }
    }

    /// <summary>
    /// Class that performs the basic functionality of a wizard
    /// </summary>
    internal class WizardHelper
    {
        #region "Private variables"

        private readonly Panel _PageHost;
        private int _SelectedPage = 0;
        private bool _AllowToGoBack = true;
        private bool _AllowToGoNext = true;

        private Panel _LabelHost = null;
        private Button _BackButton = null;
        private Button _NextButton = null;
        private Button _CancelButton = null;
        private Button _FinishButton = null;

        #endregion

        #region "Class initializer"

        /// <summary>
        /// Initializes a new instance of the WizardHelper class
        /// </summary>
        /// <param name="pageHost">Panel which hosts the wizard pages</param>
        /// <param name="initialPageIndex">Index of page to be initially displayed</param>
        public WizardHelper(Panel pageHost, int initialPageIndex = 0)
        {
            _PageHost = pageHost;
            PageCount = _PageHost.Children.Count;

            InitializePages();
            SelectedPage = initialPageIndex;
        }

        #endregion

        #region "Public properties"

        /// <summary>
        /// Gets the count of pages in the wizard
        /// </summary>
        public int PageCount { get; private set; }


        /// <summary>
        /// Gets or sets the index of the currently selected wizard page
        /// </summary>
        public int SelectedPage
        {
            get
            {
                return _SelectedPage;
            }
            set
            {
                int previousIndex = _SelectedPage;
                _SelectedPage = value;
                SetPageSelectedValue(previousIndex, false);
                SetPageSelectedValue(value, true);
                UpdateButtonsStatus();
                OnSelectedPageChanged(previousIndex, value);
            }
        }

        /// <summary>
        /// Gets or sets if the back button should be disabled on the last page
        /// </summary>
        public bool DisableBackOnFinish { get; set; } = false;


        /// <summary>
        /// Gets or sets if the user is allowed to go to the previous page
        /// </summary>
        public bool AllowToGoBack
        {
            get { return _AllowToGoBack; }
            set
            {
                _AllowToGoBack = value;
                UpdateButtonsStatus();
            }
        }

        /// <summary>
        /// Gets if the user can go to the previous page
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return _AllowToGoBack && (_SelectedPage > 0) && !(DisableBackOnFinish && CanFinish);
            }
        }


        /// <summary>
        /// Gets or sets if the user is allowed to go to the next page
        /// </summary>
        public bool AllowToGoNext
        {
            get { return _AllowToGoNext; }
            set
            {
                _AllowToGoNext = value;
                UpdateButtonsStatus();
            }
        }

        /// <summary>
        /// Gets if the user can go to the next page
        /// </summary>
        public bool CanGoNext
        {
            get
            {
                return _AllowToGoNext && (_SelectedPage < PageCount - 1);
            }
        }

        /// <summary>
        /// Gets if the user can finish the wizard (the last page is selected)
        /// </summary>
        public bool CanFinish
        {
            get
            {
                return _SelectedPage == PageCount - 1;
            }
        }

        /// <summary>
        /// Gets or sets the panel which hosts page labels
        /// </summary>
        public Panel LabelHost
        {
            get { return _LabelHost; }
            set
            {
                _LabelHost = value;
                InitializeCorrespondingLabels();
                SetLabelSelectedValue(_SelectedPage, true);
            }
        }

        /// <summary>
        /// Gets or sets the back button
        /// </summary>
        public Button BackButton
        {
            get { return _BackButton; }
            set
            {
                _BackButton = value;
                UpdateButtonsStatus();
            }
        }

        /// <summary>
        /// Gets or sets the next button
        /// </summary>
        public Button NextButton
        {
            get { return _NextButton; }
            set
            {
                _NextButton = value;
                UpdateButtonsStatus();
            }
        }

        /// <summary>
        /// Gets or sets the cancel button
        /// </summary>
        public Button CancelButton
        {
            get { return _CancelButton; }
            set
            {
                _CancelButton = value;
                UpdateButtonsStatus();
            }
        }

        /// <summary>
        /// Gets or sets the finish button
        /// </summary>
        public Button FinishButton
        {
            get { return _FinishButton; }
            set
            {
                _FinishButton = value;
                UpdateButtonsStatus();
            }
        }

        #endregion

        #region "Public events"

        public event EventHandler<SelectedPageChangedEventArgs> SelectedPageChanged;

        #endregion

        #region "Public methods"

        /// <summary>
        /// Go to the previous page
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                SelectedPage--;
            }
        }

        /// <summary>
        /// Go to the next page
        /// </summary>
        public void GoNext()
        {
            if (CanGoNext)
            {
                SelectedPage++;
            }
        }

        #endregion

        #region "Private methods"

        /// <summary>
        /// Event handler for the SelectedPageChanged event
        /// </summary>
        private void OnSelectedPageChanged(int previousIndex, int currentIndex)
        {
            SelectedPageChanged?.Invoke(this, new SelectedPageChangedEventArgs(previousIndex, currentIndex));
        }

        /// <summary>
        /// Maps a boolean to a Visibility enum
        /// </summary>
        /// <param name="visible">Boolean value corresponding to visibility</param>
        /// <param name="hiddenInsteadOfCollapsed">Use Visibility.Hidden as false state instead of Visibility.Collapse</param>
        /// <returns></returns>
        private Visibility BoolToVisibility(bool visible, bool hiddenInsteadOfCollapsed = false)
        {
            return visible ? Visibility.Visible :
                hiddenInsteadOfCollapsed ? Visibility.Hidden : Visibility.Collapsed;
        }

        /// <summary>
        /// Sets if a page is selected or not
        /// </summary>
        /// <param name="index">Index of page</param>
        /// <param name="selected">Selected value</param>
        private void SetPageSelectedValue(int index, bool selected)
        {
            UIElement p = _PageHost.Children[index];

            p.Visibility = selected ? Visibility.Visible : Visibility.Collapsed;

            SetLabelSelectedValue(index, selected);
        }

        /// <summary>
        /// Update button enabled/visibility values
        /// </summary>
        private void UpdateButtonsStatus()
        {
            if (BackButton != null)
                BackButton.IsEnabled = CanGoBack;

            if (NextButton != null)
                NextButton.IsEnabled = CanGoNext;

            if (FinishButton != null)
            {
                FinishButton.Visibility = BoolToVisibility(CanFinish);
                if (CancelButton != null)
                    CancelButton.Visibility = BoolToVisibility(!CanFinish);
            }
        }

        /// <summary>
        /// Sets if the page label is selected or not (font bold or regular)
        /// </summary>
        /// <param name="index">Index of page</param>
        /// <param name="selected">Selected value</param>
        private void SetLabelSelectedValue(int index, bool selected)
        {
            if (LabelHost == null)
                return;

            Label l = LabelHost.Children[index] as Label;
            l.FontWeight = selected ? FontWeights.Bold : FontWeights.Regular;
        }

        /// <summary>
        /// Initialize all the pages in the container, and create corresponding labels if LabelHost is set
        /// </summary>
        private void InitializePages()
        {
            foreach (UIElement control in _PageHost.Children)
            {
                control.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Create the corresponding label for each page (text from the page's Tag property)
        /// </summary>
        private void InitializeCorrespondingLabels()
        {
            if (LabelHost == null)
                return;
            foreach (UIElement page in _PageHost.Children)
            {
                FrameworkElement fwElem = page as FrameworkElement;
                Label l = new Label
                {
                    Content = fwElem.Tag as string
                };
                LabelHost.Children.Add(l);
            }
        }

        #endregion
    }
}
