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
	[Activity (Label="Debenhams", Theme = "@android:style/Theme.Light.NoTitleBar")]	
	public class ActRPoList : Activity
	{
		private EditText txtsearch;
		private ListView lvpo;
		private Button btnsearch;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			ApiRPolist ();
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayRPoList);

			var menuButton = FindViewById (Resource.Id.menuHdrButton);
			menuButton.Click += (sender, e) => {
				leftdrawer();
			};
			#region >>>>>>>>>>MENU<<<<<<<<<<
			var MenuReceiveing = FindViewById (Resource.Id.MenuReceiving);
			MenuReceiveing.Click += (sender, e) => {leftdrawer ();};

			var MenuPicking = FindViewById (Resource.Id.MenuPicking);
			MenuPicking.Click += (sender, e) => 
			{
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActPTLList));
				StartActivity (intent);
			};
			#endregion

				
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




			//var MenuPicking = FindViewById (Resource.Id.MenuPicking);
			//MenuPicking.Click += (sender, e) => 
			//{
			//	var intent= new Intent ();
			//	intent.SetClass (this, typeof(ActPTLList));
			//	StartActivity (intent);
			//};

		}
			

		public async void ApiRPolist()
		{
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Downloading Data From Server...", true);
			try
			{
				await ApiConnection1.ApiPoRList(GlobalVariables.GlobalUrl +"/RPoList/"+ GlobalVariables.GlobalUserid);
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
			var items = ((WMSApplication)Application).ItemRepository.GetRPoList(txtsearch.Text);
			lvpo.Adapter = new AdpRPoList(this, items);
		}

		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpRPoList)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			intent.SetClass(this, typeof(ActRPoScanList));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("po_num", item.po_num);
			intent.PutExtra("receiver_num", item.receiver_num);
			intent.PutExtra("division_id", item.division_id);
			intent.PutExtra("division", item.division);
			intent.PutExtra("slot_num", item.slot_num);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

	}
}

