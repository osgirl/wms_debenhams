
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
	[Activity (Label = "ActRPoScanList", Theme = "@android:style/Theme.Light.NoTitleBar")]			
	public class ActRPoScanList : Activity
	{
		private Boolean scan=true;
		private ListView lvUpc;

		private EditText txtScanUPC,txtponum,txtdivision,txtslot;

		private Button btnScanSlot,btnScanSKU,btnDone;

		//private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
		//private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type"; 
		//private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";
		//private static string ourIntentAction = "barcodescanner.RECVR";
		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		tblPoList PoList = new tblPoList();
		tblPoListDetail PoDetails = new tblPoListDetail();


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayRPoScanList);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);
			txtScanUPC = FindViewById < EditText> (Resource.Id.txtScanUPC);
			txtponum = FindViewById < EditText> (Resource.Id.txtponum);
			txtdivision = FindViewById < EditText> (Resource.Id.txtdivision);
			txtslot = FindViewById < EditText> (Resource.Id.txtslot);
			btnScanSKU = FindViewById<Button> (Resource.Id.btnScanSKU);
			btnScanSlot = FindViewById<Button> (Resource.Id.btnScanSlot);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);

			btnScanSlot.Click += new EventHandler (btnScanSlot_Clicked);
			btnScanSKU.Click += new EventHandler (btnScanUPC_Clicked);
			btnDone.Click += new EventHandler (btnDone_Clicked);
			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUPC.AfterTextChanged += delegate {ScanUPC();};

			txtponum.Text =  Intent.GetStringExtra("po_num");
			txtdivision.Text = Intent.GetStringExtra("division"); 
			txtslot.Text = Intent.GetStringExtra("slot_num");


		}
		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}
			
		private void refreshItems()
		{
			var items = ((WMSApplication)Application).ItemRepository.GetRPoListDetail (Intent.GetStringExtra ("receiver_num").ToString (),Intent.GetStringExtra ("division_id").ToString ());
			lvUpc.Adapter = new AdpRPoListScan (this, items);
		}

		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpRPoListScan)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActRPoUpc));
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("ponum", txtponum.Text);
			intent.PutExtra("receiver_num",Intent.GetStringExtra ("receiver_num").ToString());
			intent.PutExtra("division_id", Intent.GetStringExtra ("division_id").ToString ());
			intent.PutExtra("division", txtdivision.Text);
			intent.PutExtra("slot", txtslot.Text);
			intent.PutExtra("upc", item.upc);
			intent.PutExtra("description", item.description);
			intent.PutExtra("oqty", item.oqty);
			intent.PutExtra("rqty", item.rqty);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

		private void btnScanUPC_Clicked(object sender, EventArgs e)
		{ 
			if (txtslot.Text != "") 
			{
				scan = true;
				Scanning ();
				txtScanUPC.Text ="456\n";
			}
			else
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("Debenhams");
				builder.SetMessage("Please scan Slot First.");
				builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
				builder.Show ();
			}
		}
		private void btnScanSlot_Clicked(object sender, EventArgs e)
		{
			scan = false;
			txtslot.Text = "";
			Scanning ();
			txtScanUPC.Text="123\n";
		}
			

		private void Scanning()
		{
			txtScanUPC.RequestFocus();
			txtScanUPC.Text = "";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanUPC()
		{
			string oqty,rqty;

			if (Convert.ToInt32 (txtScanUPC.Text.Length) > 1) 
			{
				if (txtScanUPC.Text.Substring (Convert.ToInt32 (txtScanUPC.Text.Length) - 1, 1)=="\n") 
				{
					if (txtScanUPC.Text != ("")) 
					{
						if (scan == true) {
							var scanItem = ((WMSApplication)Application).ItemRepository.GetRPoUPC (Intent.GetStringExtra("receiver_num"),Intent.GetStringExtra("division_id"),txtScanUPC.Text);
							if (scanItem != null) {
								oqty = scanItem.oqty;
								rqty = scanItem.rqty;

								string stat = "0";
								if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
									stat = "1";
								}
								PoDetails.id = scanItem.id;
								PoDetails.receiver_num = scanItem.receiver_num;
								PoDetails.division_id = scanItem.division_id;
								PoDetails.upc = scanItem.upc;
								PoDetails.description = scanItem.description;
								PoDetails.oqty = oqty;
								PoDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
								PoDetails.status = stat;

								((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (PoDetails);
								refreshItems ();
							} 
							else 
							{
								var builder = new AlertDialog.Builder(this);
								builder.SetTitle ("Debenhams");
								builder.SetMessage ("You scanned a UPC not in the P.O.\n\nUPC: "+txtScanUPC.Text+"\nDo you want to add UPC in P.O?");
								builder.SetPositiveButton("Yes", AddInvalidUPC_Clicked);
								builder.SetNegativeButton("No",delegate { builder.Dispose(); });
								builder.Show ();
							}
						} 
						else 
						{
							txtslot.Text = txtScanUPC.Text.Substring (0, Convert.ToInt32 (txtScanUPC.Text.Length - 1));
							PoList.id = Convert.ToInt32 (Intent.GetStringExtra ("id"));
							PoList.po_num = Intent.GetStringExtra ("po_num");
							PoList.receiver_num = Intent.GetStringExtra ("receiver_num");
							PoList.division_id = Intent.GetStringExtra ("division_id");
							PoList.division = Intent.GetStringExtra ("division");
							PoList.slot_num = txtslot.Text;
							PoList.status = "In Process";
							((WMSApplication)Application).ItemRepository.UpdateRPoListSlot (PoList);
						}
					}
				}
			}
		}
		
		private void btnDone_Clicked(object sender, EventArgs e)
		{
			if (txtslot.Text != "") 
			{
				var builder = new AlertDialog.Builder(this);
				var chkitem =((WMSApplication)Application).ItemRepository.ChkRPoListVariance (Intent.GetStringExtra ("receiver_num"),PoList.division_id = Intent.GetStringExtra ("division_id"));
				if (chkitem.Count()==0) 
				{
					builder.SetTitle("Debenhams");
					builder.SetMessage("Are You Sure You Want To Close This PO?");
				} 
				else
				{
					string upc = "";
					foreach (var i in chkitem) 
					{
						upc=upc+i.upc+"\n";
					}
					builder.SetTitle("Debenhams"  );
					builder.SetMessage ("Are You Sure You Want To Close This PO?\nThere are still variance with the other following UPC/s\n\n"+ upc);
				}
				builder.SetPositiveButton("Yes", YesDialog_Clicked);
				builder.SetNegativeButton("No", delegate { builder.Dispose(); });
				builder.Show ();
			}
			else
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("Debenhams");
				builder.SetMessage("Please scan Slot First.");
				builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
				builder.Show ();
			}
		}

		private void AddInvalidUPC_Clicked(object sender, DialogClickEventArgs args)
		{
			ItemRepository.AddRPoListDetail (
				Intent.GetStringExtra ("receiver_num"),
				Intent.GetStringExtra("division_id"),
				txtScanUPC.Text.Substring (0, Convert.ToInt32 (txtScanUPC.Text.Length - 1)),
				"Not Available",
				"0",
				"1",
				"0"
			);
			refreshItems ();
		}


		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating Po...", true);
			try
			{

				var updateupc=((WMSApplication)Application).ItemRepository.UpdateRPoListDetail (Intent.GetStringExtra("receiver_num"),Intent.GetStringExtra("division_id"));
				foreach (var i in updateupc) 
				{
					if (Convert.ToInt32 (i.oqty) != 0) 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RPoListDetailUpdate/" + i.receiver_num + "/" + i.division_id + "/" + i.upc + "/" + i.rqty +"/qwde"));
					} else 
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/RPoListDetailAdd/" + i.receiver_num + "/" + i.division_id + "/" + i.upc + "/" + i.rqty +"/"+txtslot.Text +"/"+ GlobalVariables.GlobalUserid));
					}
				}
				//await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "RPoListUpdate/"+));
				tblPoList item = new tblPoList();
				var polist = ((WMSApplication)Application).ItemRepository.GetRPoListFirst (Intent.GetStringExtra ("receiver_num"),Intent.GetStringExtra("division_id"));
				item.id = polist.id;
				((WMSApplication)Application).ItemRepository.DeleteRPoList (item);
				progressDialog.Cancel ();
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle("Debenhams");
				builder.SetMessage("PO Successfully Closed");
				builder.SetPositiveButton("OK", Closed_Clicked);
				builder.Show ();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update Po.\n" + ex.Message, ToastLength.Long).Show ();
			}
		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			Finish ();
		}
	}
}

	