﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecipeManagerXamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryPage : ContentPage
	{
        #region PROPERTIES
        private Category category; // Stores the category being displayed.
        public List<Recipe> RecipeList; // List of recipes.
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Constructor for the Category Page class.
        /// </summary>
        /// <param name="category">Category</param>
        public CategoryPage (Category category)
		{
            BindingContext = this;

			InitializeComponent();

            this.category = category;

            this.Title = category.Name;

            // Sets the items source of the list view.
            ListViewRecipes.ItemsSource = RecipeList;

            // Sets the item selected listener for the list view.
            ListViewRecipes.ItemSelected += ListViewRecipes_ItemSelected;

            // Sets the item tapped listener for the list view.
            ListViewRecipes.ItemTapped += ListViewRecipes_ItemTapped;

            // Sets the clicked listener for the toolbar item.
            ToolbarItemDeleteCategory.Clicked += ToolbarItemDeleteCategory_Clicked;

            // Sets the clicked listener for the toolbar item.
            ToolbarItemAddRecipe.Clicked += ToolbarItemAddRecipe_Clicked;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Event handler for when the page appears.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Changes the colour of the navigation bar.
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#008577");

            // Gets all the recipes from the database.
            RecipeList = App.Database.GetRecipes(category);

            // Sets the items source of the list view.
            ListViewRecipes.ItemsSource = RecipeList;
        }
        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Item selected listener for the list view.
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event</param>
        private void ListViewRecipes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Sets the item to -1 so highlighting is disabled.
            ListViewRecipes.SelectedItem = -1;
        }

        /// <summary>
        /// Tapped listener for the list view.
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event</param>
        private async void ListViewRecipes_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Stores the category tapped by the user.
            var recipeTapped = e.Item as Recipe;

            // If there are no pages on the navigation stack and the last page is not a RecipePage a RecipePage will
            // be added to the stack.
            if (Navigation.NavigationStack.Count == 0 || Navigation.NavigationStack.Last().GetType() != typeof(RecipePage))
            {
                // Adds a page to the navigation stack.
                await Navigation.PushAsync(new RecipePage(recipeTapped));
            }           
        }

        /// <summary>
        /// Clicked listener for the toolbar item.
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event</param>
        private async void ToolbarItemAddRecipe_Clicked(object sender, EventArgs e)
        {
            // Adds a page to the navigation stack.
            await Navigation.PushAsync(new AddRecipePage(category.ID));
        }

        /// <summary>
        /// Clicked listener for the toolbar item.
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event</param>
        private async void ToolbarItemDeleteCategory_Clicked(object sender, EventArgs e)
        {
            // Gets the answer from the alert.
            bool answer = await DisplayAlert("Delete Category", "Are you sure you want to delete this category?", "Yes", "No");

            // Deletes the category if the user chooses "yes".
            if(answer)
            {
                if(App.Database.DeleteCategory(category.ID) == 1)
                {
                    // Removes the page from the navigation stack.
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Error deleting category", "OK");
                }
            }
        }
        #endregion
    }
}