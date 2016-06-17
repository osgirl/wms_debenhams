
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
using Debenhams.Models;
using Debenhams.DataAccess;

namespace Debenhams.Activities
{
	[Activity (Label = "ActPTLUpc")]			
	public class ActPTLUpc : Activity
	{
		private EditText txtStoreName,txtUPC,txtDescr,txtBoxNo,txtOQty,txtRqty;
		private Button btndone,btnqtyminus;

		tblPickingListDetail PTLDetails =new tblPickingListDetail();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayPTLUpc);



			txtStoreName = FindViewById<EditText> (Resource.Id.txtStoreName);
			txtUPC = FindViewById<EditText> (Resource.Id.txtUPC);
			txtDescr = FindViewById<EditText> (Resource.Id.txtDescr);
			txtBoxNo = FindViewById<EditText> (Resource.Id.txtBoxNo);
			txtOQty = FindViewById<EditText> (Resource.Id.txtOQty);
			txtRqty = FindViewById<EditText> (Resource.Id.txtRQTY);

			btndone = FindViewById<Button> (Resource.Id.btnDone);
			btnqtyminus = FindViewById<Button> (Resource.Id.btnqtyminus);

			btndone.Click += new EventHandler (btndone_Clicked);
			btnqtyminus.Click += new EventHandler (btnqty_Clicked);
			ViewDetails ();
		}

		private void ViewDetails()
		{
			txtStoreName.Text = Intent.GetStringExtra("store_name");
			txtUPC.Text = Intent.GetStringExtra("upc");
			txtDescr.Text = Intent.GetStringExtra("descr");
			txtBoxNo.Text = Intent.GetStringExtra("box_code");
			txtOQty.Text = Intent.GetStringExtra("oqty");
			txtRqty.Text = Intent.GetStringExtra("rqty");
		}

		private void btnqty_Clicked(object sender, EventArgs e)
		{
			tblBoxDetail boxdetails = new tblBoxDetail();
			if (Convert.ToInt32 (txtRqty.Text) >= 1) 
			{	
				var boxdetail = ItemRepository.ChkBoxDetailUPC (txtBoxNo.Text,Intent.GetStringExtra ("move_doc"),txtUPC.Text);
				int upccount = 0;
				if (boxdetail != null) upccount = Convert.ToInt32(boxdetail.rqty);
				if (upccount > 0 ) 
				{
					txtRqty.Text = Convert.ToString( Convert.ToInt32 (txtRqty.Text) - 1);
					string stat = "0";	
					if (Convert.ToInt32 (txtRqty.Text) == Convert.ToInt32 (Intent.GetStringExtra ("oqty"))) stat = "1";

					PTLDetails.id = Convert.ToInt32(Intent.GetStringExtra ("id"));
					PTLDetails.move_doc = Intent.GetStringExtra ("move_doc").ToString ();
					PTLDetails.upc =Intent.GetStringExtra ("upc").ToString ();
					PTLDetails.sku =Intent.GetStringExtra ("sku").ToString ();
					PTLDetails.dept = Intent.GetStringExtra ("dept").ToString ();
					PTLDetails.style = Intent.GetStringExtra ("style").ToString ();
					PTLDetails.descr =Intent.GetStringExtra ("descr").ToString ();
					PTLDetails.oqty =Intent.GetStringExtra ("oqty").ToString ();
					PTLDetails.rqty =txtRqty.Text;
					PTLDetails.status = stat;
					ItemRepository.UpdatePTLListDetail (PTLDetails);

					boxdetails.id = boxdetail.id;
					boxdetails.box_code = boxdetail.box_code;
					boxdetails.move_doc = boxdetail.move_doc;
					boxdetails.upc = boxdetail.upc;
					boxdetails.rqty = Convert.ToString (Convert.ToInt32 (boxdetail.rqty) - 1);
					ItemRepository.UpdateBoxDetail (boxdetails);
				}
				else
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle("Debenhams");
					builder.SetMessage("Unable to continue..\nThere are no UPC ("+ txtUPC.Text +")\nIn the selected Box ("+ txtBoxNo.Text +")..");
					builder.SetPositiveButton("OK", delegate { builder.Dispose(); });
					builder.Show ();
				}
			}
		}

		private void btndone_Clicked(object sender, EventArgs e)
		{	
			Finish ();
		}


	}
}

