using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Security.Cryptography;

// Json用参照ライブラリ
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;



namespace QBCodePay
{
    public partial class QBCodePay : Form
    {
        #region "インスタンス"
        /// <summary>
        ///  QRコード支払クラス
        /// </summary>
        PayClass payClass;
        #endregion


        public QBCodePay()
        {
            // QRコード支払クラス初期化
            payClass = new PayClass();

            InitializeComponent();
            // Config JSONファイルを読込(環境定義)
            if (!payClass.GetConfigJson())
            {
                // JSONファイル読込失敗はプログラム終了
                Application.Exit();
            };
            // エラー一覧取得
            if (!payClass.GetErrorList())
            {
                // JSONファイル読込失敗はプログラム終了
                Application.Exit();
            }
        }
        #region "Form Events"
        /// <summary>
        /// POSTボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPost_Click(object sender, EventArgs e)
        {
            payClass.pUrl = this.txtEndPint.Text;
            // POST METHOD実行
            payClass.Post_Method();
            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();

        }
        /// <summary>
        /// GETボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGets_Click(object sender, EventArgs e)
        {
            payClass.pUrl = this.txtEndPint.Text;
            var i = this.cbxRefund.Checked ? 1 : this.cbxTradeList.Checked ? 2 : 0;
            // GET METHOD実行
            payClass.Get_Method(i);
            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();
        }
        /// <summary>
        /// PUTボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPuts_Click(object sender, EventArgs e)
        {
            payClass.pUrl = this.txtEndPint.Text;
            // PUT METHOD実行
            payClass.Put_Method(this.cbxRefund.Checked ? 1 : 0);

            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();

        }
        #endregion
    }
}
