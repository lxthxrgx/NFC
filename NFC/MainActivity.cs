using Android.Content;
using AndroidX.AppCompat.App;
using Android.OS;
using Android.Widget;
using System;
using Android.App;
using Android.Nfc;
using System.Collections.Generic;
using Xamarin.Essentials;
namespace NFC
{
    [Activity(Label = "NFC", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        string tech_;
        private NfcAdapter nfcAdapter;
        private PendingIntent pendingIntent;
        private Queue<Intent> intentQueue = new Queue<Intent>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            Button readNfcButton = FindViewById<Button>(Resource.Id.read_nfc_button);
            Button infoNfcButton = FindViewById<Button>(Resource.Id.info_nfc_button);
            Button card_emulator_nfc_button = FindViewById<Button>(Resource.Id.em_nfc_button);

            readNfcButton.Click += ReadNfcButton_Click;
            infoNfcButton.Click += InfoNfcButton_Click;
            card_emulator_nfc_button.Click += EmulatorCard_Click;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            intentQueue.Enqueue(intent);
        }

        private async void ReadNfcButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("WaitForNfcTagAsync start");
            await NfcHelper.WaitForNfcTagAsync(this, nfcAdapter, pendingIntent, intentQueue);
        }

        private void InfoNfcButton_Click(object sender, EventArgs e)
        {
            SQLiteActions sQLiteActions = new SQLiteActions();
            sQLiteActions.ReadFromDB();
        }

        private async void EmulatorCard_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("EmulatorCard_Click start");

        }

    }
}