using System;
using Android.App;
using Android.Content;
using Android.Nfc.Tech;
using Android.Nfc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
namespace NFC
{
    public static class NfcHelper
    {
        public static async Task WaitForNfcTagAsync(MainActivity mainActivity, NfcAdapter nfcAdapter, PendingIntent pendingIntent, Queue<Intent> intentQueue)
        {
            string tech_ = null;
            try
            {
                var techLists = new string[][] {
                    new string[] { "android.nfc.tech.Ndef" },
                    new string[] { "android.nfc.tech.MifareClassic" },
                    new string[] { "android.nfc.tech.MifareUltralight" },
                    new string[] { "android.nfc.tech.NfcA" },
                    new string[] { "android.nfc.tech.NfcB" },
                    new string[] { "android.nfc.tech.NfcF" },
                    new string[] { "android.nfc.tech.NfcV" }
                };

                var filters = new IntentFilter[] { new IntentFilter(NfcAdapter.ActionTechDiscovered) };

                nfcAdapter.EnableForegroundDispatch(mainActivity, pendingIntent, filters, techLists);

                System.Diagnostics.Debug.WriteLine("WaitForNfcTagAsync nfcAdapter Start");
                Intent intent = await Task.Run(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Waiting for NFC tag");

                    while (intentQueue.Count == 0)
                    {
                    }

                    return intentQueue.Dequeue();
                });
                if (NfcAdapter.ActionTechDiscovered.Equals(intent.Action))
                {
                    System.Diagnostics.Debug.WriteLine("NFC Detected");
                    var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
                    if (tag != null)
                    {
                        var techList = tag.GetTechList();

                        foreach (var tech in techList)
                        {
                            System.Diagnostics.Debug.WriteLine("Supported Technology: " + tech);
                            tech_ = tech;
                        }
                        Ndef ndef = Ndef.Get(tag);
                        if (ndef != null)
                        {
                            ndef.Connect();
                            NdefMessage message = ndef.NdefMessage;

                            if (message != null)
                            {
                                NdefRecord[] records = message.GetRecords();
                                foreach (var record in records)
                                {
                                    byte[] payload = record.GetPayload();
                                    string textData = Encoding.UTF8.GetString(payload);
                                    System.Diagnostics.Debug.WriteLine("NDEF Text Data: " + textData);
                                }
                            }
                            ndef.Close();
                        }
                        string tagId = BitConverter.ToString(tag.GetId());
                        System.Diagnostics.Debug.WriteLine("NFC Tag UID: " + tagId);
                        SQLiteActions sQLiteActions = new SQLiteActions();
                        sQLiteActions.SaveToDB("Rfid", tagId, tech_);
                    }
                }
                nfcAdapter.DisableForegroundDispatch(mainActivity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR!!!!! ---->" + ex);
            }
        }
    }
}