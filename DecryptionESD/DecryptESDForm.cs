using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DecryptionESD
{
    public partial class DecryptESDForm : Form
    {
        public DecryptESDForm()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName))
            {
                txtEsdPath.Text = openFileDialog1.FileName;
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string esdPath = txtEsdPath.Text.Trim();
            if (string.IsNullOrEmpty(esdPath)) { MessageBox.Show("请选择ESD文件！"); return; }
            //Application .StartupPath 
            string esdKey = string.Empty;
            if (comboBox1.SelectedIndex == 1) { esdKey = "BwIAAACkAABSU0EyAAgAAAEAAQCb7Jceg+YeJXNdb7HHJ0irxNsGSWu7itcuEQkfS+znxm6XwxmfINt8SGzbIIka2eOB2t9L0lGwSM0uP3UPyhBzzc8FL735OL+RnimL4SVKDb5AsYpREOcNQgKsk6OOeo8q8+4+swvwfe6+VloNqCrjiE6bCS7TrC+haV+eabj1QaT+aSXNWrukmrvi1VFoQIVeet5BqHzciVV+bv3/iSG/EEkxV6Yqq4Y2o9bvSDIbE+lGc1bKPlT9zy+lYx+WMB0Nfzo7nIrKs7qCw8GbeRTsHo5GMWxrLNltFsDpoO0C62pSvxEGB/id2TwESrd7brudppjjJ+LdbCBUNam6zx2lhZmjconDvvWLYC6KXVVgTh5WHjv8z0dxkD+Hc6o6OhdXuxAA5xtZYgIah8t2ZVK5V2PEFnusqZP7fqbSUJOp6sZe3AZWWVZz6dg6VqYpDMbKBz8rhHXXHjkaqIMrmxnSmHoB4fsxelWre9oxQoQJUkASAUhflDPKtFVe30oLsN6fNwBBNVKywJogPsClqIuNiQDpsXRFg8PYBgvqDQz8DRfDpu5WyhQjdD+eVQpeczWmTyPuwfGB0TscKDhzIhSwebKK0NwCn2LunmdJEOjJsnsYLrE63rsQdcXimzPifQ5XWlV9GUqo5ce+AlX0IMmw7DSZJBe9Sr1adBFuHQvRvQ1tGyQ5oD7WxKshS8WbvKT6cZb0XBE0Ru82gl9uSvJAgOJG9E8g7BApwCfaWAMEVj/Xd8DZSjZ0VWxRlsVkhjxWeiiQWg85J08JdjC2soG14IiXRTVAGogUTcUVlOPkovrWoRVqTMLAA+Vh0R6BpcAexwUv4YVmw2661iDUmnkYWyXMcSBQP43h5SjdLirO19b/1UD9lvaCaZpyokKMD6+GNJyCX9stVuS7c1ow/nsuVDgx3Rv7wE0as5h9WSheM5p6Lzf1UTHy1Mg2XpJAn37amw7rUnOkz4qqJ5ItRwAAhRn2Cn+PUtp5Ti1vfHmPd0PodAdTUo+5lYOGmXJxx0SHTh3dSCkSIJWjoAFYnrtEWMTszcenxYlZc2e6dWNZUPO4VyftrGkF4FpWxFC63Gd15uf3vUlStFXRArMh9KQ14sf4PGmpxmoXNZBWNsa7xizW83lkS3sbtnHtLEk6xyJ01sj+HWPoEuTzSbs6v3P8NstpN37xJui+hafIA5ILpB04qlxrxIRKEow31QzrMINyiwjCnxzIrXFuEQq9aY1930q5XgYfoV8OZirdIWKYtvPWzBgEkmi5w930kqjRyA81XwhH74guWww7A1lImbygBDl5wyE8GRVg2Emy7pU2sybyvtjMLSBgmTK8h6UqEXaupvuVCYjC8BzkUGWfTG9eh50TPpFZMO4vB2l2tbfwA2oTBwvjuwaDwdUIFXYjmdti3A+EGuhvDAeGzGxZhOmQir84CYXEKMj4yaqFvaKecMCtOOwWWpAEIzRWxXJIXN6EPqiZGEauZUAXjicVI/jpkKGhWUnaLZhNFTYD6Nl/eBMA2TPj6w4AiOnr21bDjgE="; }
            else if (comboBox1.SelectedIndex == 0) { esdKey = "BwIAAACkAABSU0EyAAgAAAEAAQA5WQn9lTT4Ci679UcfZW6y8GkbeGTN9bKbgjnigtWmb7pPkifbr3ihmmtJ1ZWJmZCyDyeRNHwHDieOiM8zfgRJr575RKQI8yWi6wNAZVhUZDzKlch4BgABut2lcjZRT5o/Iyotd0tKW7Np1ur8D/HawdmHXdhN42hjg7PKNfvbgXICNNK+uschVzA39HWejEowO5ppaXBObLpN28Ipun3+s0xPNePHNVKD/4azFGd11ZZWmoh3NpnZXBGW3Jk2fn6hmrQ434Mrw4qdpIfTqh/d0aWGE7CseZPYR0F9Gd6DWyXn6JzkvPBPRNtdU7SK5Xeh+pDmTnme5av3c1XNBka2hScgqAT/BOAwaIvufA6QZXccHkeHnOVO/XHEi339OT0FJLNWearerYzfHtHh6D8+d7fIdHBsgMCMd/O2suhNVBWsipzA8UPnhy4+4uPAhoV7fqaYjPbE0fUTXT82SUG11W4tjs8+kTflzwX1qoNezfLdG6++h7LJSGJNPe2QfsQlB8NxLTReIHsyW5Fv5Q0LZH/Z5tJsOeu0P39z9k/oW30TGHIVnipOkdfA1PREFFwDWJ7MKsTQkW2ikSo9Y1HbUhIAb2xI9M28GAxGxdaPa69vAepfqiPOfEFOiZcUhVkLs6vv8GVOsLRMHFalKhwNi6bWX32R76OKmRHLPPl47dkHCBy/nBVSLaVyUo25gEX3pVgGDpoiHOTzeq1qvPdguBMXHtgvpvEMTBEMDFpp1hMqWkNcapPA25oGQmJr5LouRsfaHXe52LoJpniCA/Lf7cFSCbx+Wkh1bl/4uepz45bZGpjde4WvPnKPBOsi+EZ30lYi0mfKGBQ7HS6RE9iQSbOJYZ2djnY+ok8VkGrXU28l1kQParu3mnXOcQdviIJhtH6nor3GjXYbMml40/b3lGPn6qPjf0UW9glD2apdQMyTTxO2YzLlpiW96d5SwsPTDfP83YTZUZd6Er4cvmlb7G4qidlF7xIdVzzmGx5PPAuv6oLzMf3qFHKgo8nGC3ZcHfTsHz62eTvDFfCxuqTSbZYby+SraGvez3gZSKnbmvkfaBumgMPULGjsPC0FGMf1PXzxHQ3Y5chnpxYXF86h9NRRf9efeByhj3cS1AQGNidgIfo1l0CdNDtWegcZC/0U8+0O/lMGUnemt8a+Zl6jb+XHB9czxWjfetE3KcLXlfXrIBMM7Ve3JNEU1dL01vZ7THJXYWS6mIvGnOK+nW4GxsgReW8an5HlE1qF3O0r0vmpttZ6tK0NjxZFrUIVJwE+X/rJrRIS7eJJsgLoI4HD37AMcQ3rGY4/mnR7JitqNj4TNq+P/XNNl7wkjmRLOruLrOdShKON1ZvmaZ9BKUYI02FjxRntO8MPOrR2ImdRpTp+1rGtLlWWe0MxmPOkIQIsPKocIeitjWXIgNErcdzulagizd+cmcf2PPOyNkOd7yVv1xxxLy2ePYsHdGaYxIgM0xJ+NNrNpLz9/3W2quhEt4JL6jIhnIuvIUd67SQLwf7qy2jS3lLwbkBqPJpalAE="; }
            //Process p = new Process();
            MessageBox.Show(Application.StartupPath);
            Process p = Process.Start(Application.StartupPath + "\\Plugins\\esddecrypt.exe", esdPath + " " + esdKey);
            p.WaitForExit();
            MessageBox.Show("解密完成！");
        }

        private void DecryptESDForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}
