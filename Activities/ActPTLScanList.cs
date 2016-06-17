
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Debenhams.DataAccess;
using Debenhams.Adapter;
using Debenhams.Models;

namespace Debenhams.Activities
{
	[Activity (Label = "ActPTScanList",Theme = "@android:style/Theme.Light.NoTitleBar")]			
	public class ActPTLScanList : Activity
	{

		private ListView lvUpc;
		private EditText txtScanUPC,txtTlno,txtStoreName,txtBoxCode;
		private Button btnScanUpc,btnDone;


		tblPickingListDetail PTLDetails = new tblPickingListDetail();

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayPTLScanList);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);

			txtScanUPC = FindViewById < EditText> (Resource.Id.txtScanUPC);
			txtTlno= FindViewById < EditText> (Resource.Id.txtTlno);
			txtStoreName= FindViewById < EditText> (Resource.Id.txtStoreName);
			txtBoxCode= FindViewById < EditText> (Resource.Id.txtBoxCode);

			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnScanUpc = FindViewById<Button> (Resource.Id.btnScanUpc);

			txtTlno.Text = Intent.GetStringExtra ("move_doc");
			txtStoreName.Text = Intent.GetStringExtra ("store_name");
			txtBoxCode.Text = Intent.GetStringExtra ("box_code");

			btnScanUpc.Click += delegate {Scanning();};
			
			//btnDone.Click += new EventHandler (btnDone_Clicked);
			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUPC.AfterTextChanged += delegate {ScanUPC();};
			refreshItems ();
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetPickingListDetail (Intent.GetStringExtra("move_doc"));
			lvUpc.Adapter = new AdpPTLListScan (this, items);
		}


		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpPTLListScan)lvUpc.Adapter).GetItemDetail(e.Position);

			var intent = new Intent();
			intent.SetClass(this, typeof(ActPTLUpc));

			intent.PutExtra("box_code",Intent.GetStringExtra ("box_code"));
			intent.PutExtra("store_id", Intent.GetStringExtra ("store_id"));
			intent.PutExtra("store_name",txtStoreName.Text);
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("move_doc", txtTlno.Text);
			intent.PutExtra("upc", item.upc);
			intent.PutExtra("sku", item.sku);
			intent.PutExtra("dept", item.dept);
			intent.PutExtra("style", item.style);
			intent.PutExtra("descr", item.descr);
			intent.PutExtra("oqty", item.oqty);
			intent.PutExtra("rqty", item.rqty);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

		private void Scanning()
		{
			txtScanUPC.RequestFocus();
			txtScanUPC.Text = "";
			txtScanUPC.Text="01upc\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanUPC()
		{
			tblBoxDetail boxdetails = new tblBoxDetail();
			string oqty,rqty;
			if (Convert.ToInt32 (txtScanUPC.Text.Length) > 1) 
			{
				if (txtScanUPC.Text.Substring (Convert.ToInt32 (txtScanUPC.Text.Length) - 1, 1)=="\n") 
				{
					var scanItem = ItemRepository.GetPTLUPC (Intent.GetStringExtra ("move_doc"),txtScanUPC.Text);
					if (scanItem != null) 
					{
						oqty = scanItem.oqty;
						rqty = scanItem.rqty;

						string stat = "0";
						if (Convert.ToInt32 (rqty)+1 == Convert.ToInt32 (oqty)) {
							stat = "1";
						}
						PTLDetails.id = scanItem.id;
						PTLDetails.move_doc = scanItem.move_doc;
						PTLDetails.upc = scanItem.upc;
						PTLDetails.sku = scanItem.sku;
						PTLDetails.dept = scanItem.dept;
						PTLDetails.style = scanItem.style;
						PTLDetails.descr = scanItem.descr;
						PTLDetails.oqty = oqty;
						PTLDetails.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
						PTLDetails.status = stat;
						ItemRepository.UpdatePTLListDetail (PTLDetails);

						var boxdetail = ItemRepository.ChkBoxDetailUPC (txtBoxCode.Text,txtTlno.Text,scanItem.upc);
						if (boxdetail != null) 
						{
							boxdetails.id = boxdetail.id;
							boxdetails.box_code = txtBoxCode.Text;
							boxdetails.move_doc = txtTlno.Text;
							boxdetails.upc = scanItem.upc;
							boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) + 1);
							ItemRepository.UpdateBoxDetail (boxdetails);
						}
						else
						{
							ItemRepository.AddBoxDetail (txtBoxCode.Text, txtTlno.Text, scanItem.upc, "1");
						}

						refreshItems ();
					} 
					else
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle("Debenhams");
						builder.SetMessage("You've scan UPC not in Picking List.");
						builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
						builder.Show ();
					}
				}
			}
		}


	}
}

