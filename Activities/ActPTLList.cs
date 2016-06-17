using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams.Adapter;
using Debenhams.ApiConnection;

namespace Debenhams.Activities
{
	[Activity ( Label = "ActLogin",Theme = "@android:style/Theme.Light.NoTitleBar")]		
	public class ActPTLList : Activity
	{
		private EditText txtsearch;
		private ListView lvpo;
		private Button btnsearch;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			ApiRPolist ();
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayPTLList);

			var menuButton = FindViewById (Resource.Id.menuHdrButton);
			menuButton.Click += (sender, e) => {
				leftdrawer();
			};

			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};

			btnsearch = FindViewById<Button> (Resource.Id.btnsearch);
			btnsearch.Click += delegate {
				refreshItems();
			};
		}


		private async void ApiRPolist()
		{
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Downloading Data From Server...", true);
			try
			{
				await ApiConnection1.ApiPTLList(GlobalVariables.GlobalUrl +"/PTLList/"+GlobalVariables.GlobalUserid);
				refreshItems ();
			}
			catch(Exception ex) 
			{
				Toast.MakeText (this,"Unable To Download Data.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel();
		}


		public override void OnBackPressed()
		{
			leftdrawer ();
		}


		private void leftdrawer()
		{
			var menu = FindViewById<ActLeftDrawer> (Resource.Id.ActLeftDrawer);
			menu.AnimatedOpened = !menu.AnimatedOpened;
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ((WMSApplication)Application).ItemRepository.GetPTLList(txtsearch.Text);
			lvpo.Adapter = new AdpPTLList(this, items);
		}


		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpPTLList)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			intent.SetClass(this, typeof(ActPBox));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("move_doc", item.move_doc);
			intent.PutExtra("store_id", item.store_id);
			intent.PutExtra("store_name", item.store_name);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}


	}
}


