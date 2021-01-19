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
        #region "コンスタント群"
        /// <summary>
        /// Result_Code:PAY_SUCCESS(支払成功)
        /// </summary>
        private string cResult_Code_S = "PAY_SUCCESS";
        /// <summary>
        /// Result_Code:PAYING(支払待ち)
        /// </summary>
        private string cResult_Code_W = "PAYING";
        /// <summary>
        /// 結果コード(正常:MP10000)
        /// 以外は異常と判定する
        /// </summary>
        private string cReturnCode = "MP10000";
        #endregion

        #region "プロパティ群"
        /// <summary>
        /// API URL
        /// </summary>
        public string pUrl { 
            get {
                /*
                 * URLの末尾に「/」がある場合は削除 
                 * 先頭に「/」付きのAPI URIが後で追記されるため
                 */
                string u = sUrl;
                if ( (!string.IsNullOrEmpty(u)) && (u.Substring(sUrl.Length -1) == "/"))
                {
                    u = u.Substring(0,sUrl.Length - 1);
                }
                return u;
            }
            set { sUrl = value; }
        }
        string sUrl = string.Empty;
        /// <summary>
        /// 本番環境URL
        /// </summary>
        public string pProdHostUrl { get; set; }
        /// <summary>
        /// 動作確認環境(スタブ環境)
        /// </summary>
        public string pStubHostUrl { get; set; }
        /// <summary>
        /// 検証環境
        /// </summary>
        public string pVerifHostUrl { get; set; }
        /// <summary>
        /// GET METHOD Pauling間隔(ミリ秒)
        /// </summary>
        public int pGetPollTime { get; set; }
        /// <summary>
        /// QRコード支払いタイムアウト(MAX30秒)
        /// </summary>
        public int pCPMTimeOut { get; set; }
        /// <summary>
        /// 返金タイムアウト(MAX300秒)
        /// </summary>
        public int pRefundTimeOut { get; set; }
        /// <summary>
        /// ユーザー認証API URL
        /// </summary>
        public string pAuthUrl { get; set; }
        /// <summary>
        /// 支払API(PUT)URL
        /// </summary>
        public string pCPMUrl { get; set; }
        /// <summary>
        /// 支払確認API(GET)URL
        /// </summary>
        public string pCPMUrlGet { get; set; }
        /// <summary>
        /// 返金API(PUT)URL
        /// </summary>
        public string pRefundUrl { get; set; }
        /// <summary>
        /// 返金確認API(GET)URL
        /// </summary>
        public string pRefundUrlGet { get; set; }
        /// <summary>
        /// 取引記録照会(店舗単位-GET)API
        /// </summary>
        public string pTradeStore { get; set; }
        /// <summary>
        /// 取引記録照会(企業単位-PUT)API
        /// </summary>
        public string pTradeCorp { get; set; }
        /// <summary>
        /// アクセスポート
        /// </summary>
        public int pAccessPort { get; set; }
        #endregion
        #region "インスタンス群"
        /// <summary>
        /// QRコード支払レスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.CpmRes cpm;
        /// <summary>
        /// 支払い確認APIレスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.CpmCheck resp;
        /// <summary>
        /// ユーザー認証APIレスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.UserAuthR userAuthR;
        /// <summary>
        /// 返金API(PUT METHOD)レスポンスJSONクラスインスタンス
        /// </summary>
        MakeJsons.ReFoundRes reFoundR;
        /// <summary>
        /// 結果コード
        /// </summary>
        string rReturnCode { get; set; }
        /// <summary>
        /// 処理結果コード
        /// </summary>
        string rResultCode { get; set; }

        #endregion
        public QBCodePay()
        {
            InitializeComponent();
            // Config JSONファイルを読込(環境定義)
            if (!ReadConfigJson())
            {
                // JSONファイル読込失敗はプログラム終了
                Application.Exit();
            };
        }
        private bool ReadConfigJson()
        {
            bool rtn = false;
            try
            {
                var pt =  File.ReadAllText("./Config.json",Encoding.UTF8);
                MakeJsons.Configure jsn = new MakeJsons.Configure();
                jsn = JsonConvert.DeserializeObject<MakeJsons.Configure>(pt);
                /*
                 * 各環境変数に値を編入
                 */
                // GET METHOD Pauling間隔を設定(2秒以上)
                pGetPollTime = jsn.pollingTimeout.GetPollTime;
                // QRコード支払いTimeOut設定(30秒)
                pCPMTimeOut = jsn.pollingTimeout.CpmTimeOut;
                // 返金TimeOut設定(300秒)
                pRefundTimeOut = jsn.pollingTimeout.RefundTimeOut;
                // 本番環境URL
                pProdHostUrl = jsn.urls.ProdHostUrl;
                // 動作確認環境(スタブ環境)URL
                pStubHostUrl = jsn.urls.StubHostUrl;
                // 検証環境
                pVerifHostUrl = jsn.urls.VerifHostUrl;
                // ユーザー認証(POST) URL
                pAuthUrl = jsn.urls.PostAuthUrl;
                // QRコード支払(PUT) URL
                pCPMUrl = jsn.urls.PutOrderUrl;
                // 支払確認(GET) URL
                pCPMUrlGet = jsn.urls.GetCheckUrl;
                // 返金API(PUT)　URL
                pRefundUrl = jsn.urls.PutRefndUrl;
                // 返金確認API(GET)　URL
                pRefundUrlGet = jsn.urls.GetRefChkUrl;
                // 取引記録照会(店舗)API URL
                pTradeStore = jsn.urls.GetTrdInfStrUrl;
                // 取引記録照会(企業)API URL
                pTradeCorp = jsn.urls.PostTrdInfCrpUrl;
                // アクセスポート
                pAccessPort = jsn.urls.AccessPort;
            }
            catch (Exception e)
            {
                Console.WriteLine("Config.jsonファイルの読込失敗:{0}",e.Message);
                rtn = false;
            }
            return rtn;
        }
        /// <summary>
        /// EndPoint入力チェック
        /// </summary>
        /// <returns></returns>
        private bool ChkEndPoint(string surl)
        {
            bool rtn = true;

            try
            {
                if (!string.IsNullOrEmpty(surl))
                {
                    rtn = Regex.IsMatch(surl,
                       @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$"
                    );

                    if (rtn == false)
                    {
                        Console.WriteLine(string.Format("EndPointとして認識できない文字列です。"));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Urlを入力して下さい。"));
                    rtn = false;
                }
            }
            catch (Exception e)
            {
                rtn = false;
                Console.WriteLine(string.Format("EndPointとして認識できない文字列です。:{0}", e.Message));
            }
            finally
            {
                if (rtn == false)
                {
                    MessageBox.Show("EndPointとして認識できない文字列が入力されています。", "間違いなく間違ってます");
                }
            }
            return rtn;
        }
        #region "Form Events"
        /// <summary>
        /// POSTボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPost_Click(object sender, EventArgs e)
        {
            pUrl = this.txtEndPint.Text;
            // POST METHOD実行
            Post_Method();
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
            pUrl = this.txtEndPint.Text;
            // GET METHOD実行
            Get_Method(this.cbxRefund.Checked ? 1 : 0);
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
            pUrl = this.txtEndPint.Text;
            // PUT METHOD実行
            Put_Method(this.cbxRefund.Checked ? 1 : 0);

            this.txtEndPint.SelectAll();
            this.txtEndPint.Focus();

        }
        #endregion
        #region "Http Method"
        /// <summary>
        /// GET METHOD
        /// </summary>
        private async void Get_Method(int mode = 0)
        {
            if (!ChkEndPoint(pUrl))
            {
                return;
            }
            bool rtn = await GetApiFrmUrl(pUrl,mode);
            /* 
             * APIレスポンスのReturn_Codeが"MP10000(処理正常)"でかつメソッドリターンがfalseの場合は 
             * 支払待ちと判定し支払確認(GET METHOD)のPaulingを行う
             */
            if ((resp.ReturnCode == cReturnCode) && (rtn = false))
            {
                while (true)
                {
                    // GET METHOD PAULING
                    if (await GetPauling() == true)
                    {
                        // 処理正常でかつ支払完了ならPAULING終了
                        if ((resp.ReturnCode == cReturnCode) && (resp.Result.Result_code == cResult_Code_S))
                        {
                            // 支払完了
                            break;
                        }
                    }
                    else
                    {
                        if ((resp.ReturnCode != cReturnCode))
                        {
                            // エラー処理へ
                            break;
                        }

                    }
                }
            }

        }
        /// <summary>
        /// PUT METHOD
        /// </summary>
        private async void Post_Method()
        {
            // URL正当性チェック
            if (!ChkEndPoint(pUrl))
            {
                return;
            }
            string jdata = string.Empty;
            // ユーザー認証API[POST METHOD]用 jsonデータ生成～httpRequest送信
            if (PostAuthJson(ref jdata))
            {
                await PostApiFrmUrl(pUrl, jdata);
            }

        }
        /// <summary>
        /// PUT METHOD 
        /// </summary>
        /// <param name="mode">
        /// API選択(0:支払,1:返金)
        /// </param>
        private async void Put_Method(int mode = 0)
        {
            // URL正当性チェック
            if (!ChkEndPoint(pUrl))
            {
                return;
            }

            string jdata = string.Empty;
            // QRコード支払(CPM)API[PUT METHOD]用 jsonデータ生成～httpRequest送信
            if (PutCPMJson(ref jdata,mode))
            {
                // Put Method Request送信(jdata:JSONフォーマット)
                bool rtn = await PutApiFrmUrl(pUrl, jdata, mode);
                // メソッドリターンがfalseでAPI処理が正常の場合は支払確認処理をPAULING
                if ((!string.IsNullOrEmpty(rReturnCode)) && (!string.IsNullOrEmpty(rResultCode)))
                {
                    if ((rtn == false) && (rReturnCode == cReturnCode) && (rResultCode == cResult_Code_W))
                    {
                        while (true)
                        {
                            // GET METHOD PAULING
                            if (await GetPauling(mode) == true)
                            {
                                // 処理正常でかつ支払完了ならPAULING終了
                                if ((!string.IsNullOrEmpty(rReturnCode)) && (!string.IsNullOrEmpty(rResultCode)) &&
                                    (rReturnCode == cReturnCode) && (rResultCode == cResult_Code_S))
                                {
                                    // 支払完了
                                    break;
                                }
                            }
                            else
                            {
                                if ((!string.IsNullOrEmpty(resp.ReturnCode)) && (resp.ReturnCode != cReturnCode))
                                {
                                    // エラー処理へ
                                    break;
                                }

                            }

                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("PUT METHOD RequestJSONファイルの生成に失敗しました。", "JSONファイル生成エラー");
            }

        }
        #endregion
        /// <summary>
        /// Get Method 詳細
        /// </summary>
        /// <param name="s"></param>
        private async Task<bool> GetApiFrmUrl(string s,int mode = 0)
        {
            bool rtn = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                // クエリパラメタ追記
                var prms = new Dictionary<string, string>();
                /*
                 * GETデータ（クエリパラメータ）
                 * storeOrderId(支払伝票番号)固定20桁を設定する
                 */
                // 支払(返金)伝票番号(半角数字固定20桁)
                decimal n = 0;
                // 対象支払伝票番号(半角数字固定20桁)
                decimal nn =0;
                // HttpHeader編集
                var client = new HttpClient();
                // QRコード支払と返金処理のパラメタ設定
                if (mode == 0)
                {
                    // 支払伝票番号をセット
                    n = 11234567890123456789;
                    // 支払確認API URLとクエリパラメタをセット
                    s += pCPMUrlGet;

                    prms = new Dictionary<string, string>()
                    {
                        {"storeOrderId", n.ToString() },
                    };
                    s += "?" + await new FormUrlEncodedContent(prms).ReadAsStringAsync();
                    // タイムアウト設定
                    client.Timeout = TimeSpan.FromMilliseconds(pCPMTimeOut);
                }
                else
                {
                    // 返金伝票番号をセット
                    n = 11234567890123456789;
                    nn = 15765432109876543210;
                    // 返金確認API URLとクエリパラメタをセット
                    s += pRefundUrlGet;

                    prms = new Dictionary<string, string>()
                    {
                        {"storeRefundId", n.ToString() },
                        {"storeOrderId", nn.ToString() },
                    };
                    s += "?" + await new FormUrlEncodedContent(prms).ReadAsStringAsync();
                    // タイムアウト設定
                    client.Timeout = TimeSpan.FromMilliseconds(pRefundTimeOut);
                }
                AddHttpHeader2(ref client);
                var res = await client.GetAsync(s);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    // GetResponse取得成功
                    Console.WriteLine("GET成功！");
                    var g = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(g))
                    {
                        if (mode == 0)
                        {
                            // 支払確認API
                            resp = new MakeJsons.CpmCheck();
                            resp = JsonConvert.DeserializeObject<MakeJsons.CpmCheck>(g);
                            // 結果コード移入
                            rReturnCode = resp.ReturnCode;
                            // 処理結果コード移入
                            rResultCode = resp.Result.Result_code;
                            // 確認のためアイアログ表示
                            string rs =
                                string.Format("ReturnCode:{0} \r\n", resp.ReturnCode) +
                                string.Format("ReturnMessage:{0}\r\n", resp.ReturnMessage) +
                                string.Format("MsgSummaryCode:{0}\r\n", resp.MsgSummaryCode) +
                                string.Format("MsgSummary:{0}\r\n", resp.MsgSummary) +
                                string.Format("Result.partner_refund_id:{0}\r\n", resp.Result.partner_refund_id) +
                                string.Format("Result.Currency:{0}\r\n", resp.Result.Currency) +
                                string.Format("Result.Order_id:{0}\r\n", resp.Result.Order_id) +
                                string.Format("Result.Return_code:{0}\r\n", resp.Result.Return_code) +
                                string.Format("Result.Result_code:{0}\r\n", resp.Result.Result_code) +
                                string.Format("Result.Real_fee:{0}\r\n", resp.Result.Real_fee) +
                                string.Format("Result.Channel:{0}\r\n", resp.Result.Channel) +
                                string.Format("Result.Create_time:{0}\r\n", resp.Result.Create_time) +
                                string.Format("Result.Total_fee:{0}\r\n", resp.Result.Total_fee) +
                                string.Format("Result.Pay_time:{0}\r\n", resp.Result.Pay_time) +
                                string.Format("Result.Refund_fee:{0}\r\n", resp.Result.Refund_fee) +
                                string.Format("Result.Order_body:{0}\r\n", resp.Result.Order_body) +
                                string.Format("Result.Status:{0}\r\n", resp.Result.Status) +
                                string.Format("Result.PartialRefundFlag:{0}\r\n", resp.Result.PartialRefundFlag) +
                                string.Format("BalanceAmount:{0}\r\n", resp.BalanceAmount)
                                ;
                            //MessageBox.Show(rs, "帰ってきたパラメタ");
                            Console.WriteLine(rs);
                        }
                        else if (mode == 1)
                        {
                            // 返金確認API

                        }
                        // API正常
                        if (rReturnCode == cReturnCode)
                        {
                            // 支払完了
                            if(rResultCode == cResult_Code_S)
                            {
                                rtn = true;
                                MessageBox.Show("支払確認が完了しました。", "支払確認");
                            }
                        }
                    }
                }
                else
                {
                    // GetResponse取得失敗
                    Console.WriteLine("GET失敗！");
                    var g = res.StatusCode.ToString();
                    MessageBox.Show(g, "GET失敗のStatusCode");
                }

                // http client開放
                client.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("あかんやん！: {0}", e.Message));
                Console.WriteLine(string.Format("なんでやねん！: {0}", e.InnerException));
                MessageBox.Show(string.Format("Exceptionが発生しましたよ。:\r\n{0} \r\ninner Ex:\r\n{1}", e.Message, e.InnerException),
                    "残念ながらGETに失敗してます。");
            }

            return rtn;
        }
        /// <summary>
        /// POST METHOD 詳細
        /// </summary>
        /// <param name="s">URL</param>
        /// <param name="jdata">JSON DATA</param>
        /// <param name="mode">API選択フラグ(0:ユーザー認証,1:取引記録照会(企業単位))</param>
        /// <returns></returns>
        private async Task<bool> PostApiFrmUrl(string s,string jdata = "",int mode=0)
        {
            bool rtn = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                // API URL生成
                if (mode == 0)
                {
                    s += pAuthUrl;
                }
                else if(mode == 1)
                {
                    s += pTradeCorp;
                }
                // JSONデータ添付
                HttpContent content = new StringContent(jdata, Encoding.UTF8, "application/json");
                // HttpHeader編集
                var client = new HttpClient();
                // HttpHeaderの生成・設定
                AddHttpHeader2(ref client);
                // POST METHOD
                var res = await client.PostAsync(s, content);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("POST成功！");
                    // Response取得
                    var g = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(g))
                    {
                        // Response Jsonデシリアライズ
                        userAuthR = new MakeJsons.UserAuthR();
                        userAuthR=JsonConvert.DeserializeObject<MakeJsons.UserAuthR>(g);
                        // 確認のためアイアログ表示
                        string authres =
                            string.Format("ReturnCode:{0}", userAuthR.ReturnCode) + "\r\n" +
                            string.Format("ReturnMessage:{0}", userAuthR.ReturnMessage) + "\r\n" +
                            string.Format("MsgSummaryCode:{0}", userAuthR.MsgSummaryCode) + "\r\n" +
                            string.Format("MsgSummary:{0}", userAuthR.MsgSummary) + "\r\n" +
                            string.Format("Result.CredentialKey:{0}", userAuthR.Result.CredentialKey) + "\r\n" +
                            string.Format("Result.PartnerFullName:{0}", userAuthR.Result.PartnerFullName) + "\r\n" +
                            string.Format("Result.Description:{0}", userAuthR.Result.Description) + "\r\n" +
                            string.Format("Result.AdminPassword:{0}", userAuthR.Result.AdminPassword) + "\r\n" +
                            string.Format("Result.AuthForRefund:{0}", userAuthR.Result.AuthForRefund.ToString()) + "\r\n" +
                            string.Format("Result.CashNumber:{0}", userAuthR.Result.CashNumber) + "\r\n" +

                            string.Format("Result.WarningWord:{0}", userAuthR.Result.WarningWord) + "\r\n" +
                            string.Format("Result.CheckSnFlag:{0}", userAuthR.Result.CheckSnFlag.ToString()) + "\r\n" +
                            string.Format("Result.MerchantFullName:{0}", userAuthR.Result.MerchantFullName) + "\r\n" +
                            string.Format("Result.MerchantName:{0}", userAuthR.Result.MerchantName) + "\r\n" +
                            string.Format("Result.MerchantKanaName:{0}", userAuthR.Result.MerchantKanaName) + "\r\n" +
                            string.Format("Result.Prefectures:{0}", userAuthR.Result.Prefectures) + "\r\n" +
                            string.Format("Result.City:{0}", userAuthR.Result.City) + "\r\n" +
                            string.Format("Result.Street:{0}", userAuthR.Result.Street) + "\r\n" +
                            string.Format("Result.Address:{0}", userAuthR.Result.Address) + "\r\n" +
                            string.Format("Result.ContactPhoneNum:{0}", userAuthR.Result.ContactPhoneNum) + "\r\n" +
                            string.Format("Result.Email:{0}", userAuthR.Result.Email) + "\r\n" +
                            string.Format("Result.ContactHomeUrl:{0}", userAuthR.Result.ContactHomeUrl) + "\r\n" +
                            string.Format("Result.QrProvisionMethod:{0}", userAuthR.Result.QrProvisionMethod) + "\r\n" +
                            string.Format("Result.MerchantId:{0}", userAuthR.Result.MerchantId.ToString()) + "\r\n" +
                            string.Format("Result.PwChangedFlag:{0}", userAuthR.Result.PwChangedFlag.ToString()) + "\r\n" +
                            string.Format("Result.PayTypeList:{0}", "下記に記載") + "\r\n" +
                            string.Format("BalanceAmount:{0}\r\n", userAuthR.BalanceAmount) + 
                            string.Format("=== PayTypeList === \r\n");

                        foreach (MakeJsons.PayList list in userAuthR.Result.PayTypeList)
                        {
                            authres +=
                                string.Format("list.PayTypeId:{0} \r\n", list.PayTypeId.ToString()) +
                                string.Format("list.PayTypeCode:{0} \r\n", list.PayTypeCode) +
                                string.Format("list.PayTypeName:{0} \r\n", list.PayTypeName) +
                                string.Format("list.QrcodeRegExr:{0} \r\n", list.QrcodeRegExr) +
                                string.Format("list.DispQrcodeFlag:{0} \r\n", list.DispQrcodeFlag.ToString()) +
                                string.Format("list.ReadQrcodeFlag:{0} \r\n", list.ReadQrcodeFlag.ToString());
                        }

                        Console.WriteLine(authres, "帰ってきたjsonパラメタ");
                        rtn = true;

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("ユーザー認証(POST METHOD)でException発生:{0}", e.Message);
            }

            return rtn;
        }
        /// <summary>
        /// PUT METHOD 詳細
        /// </summary>
        /// <param name="s">URL</param>
        /// <param name="jdata">JSON DATA</param>
        /// <param name="mode">
        /// 0:支払,1:返金
        /// </param>
        private async Task<bool> PutApiFrmUrl(string s,string jdata = "",int mode = 0)
        {
            bool rtn = false;

            try
            {
                // TLS1.2指定
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // JSONデータ添付
                HttpContent content = new StringContent(jdata,Encoding.UTF8,"application/json");
                // HttpHeader編集
                var client = new HttpClient();
                // QRコード支払と返金処理の設定を行う
                if (mode == 0)
                {
                    // TimeOut
                    client.Timeout = TimeSpan.FromMilliseconds(pCPMTimeOut);
                    // QRコード支払API URLを追記
                    s += pCPMUrl;
                }
                else
                {
                    // TimeOut
                    client.Timeout = TimeSpan.FromMilliseconds(pRefundTimeOut);
                    // 返金API URLを追記
                    s += pRefundUrl;
                }
                // HttpHeaderの生成・設定
                AddHttpHeader2(ref client);
                var res = await client.PutAsync(s, content);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("PUT成功！");
                    var g = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(g))
                    {
                        if (mode == 0)
                        {
                            //   支払API使用時
                            cpm = new MakeJsons.CpmRes();
                            cpm = JsonConvert.DeserializeObject<MakeJsons.CpmRes>(g);
                            // 結果コード移入
                            rReturnCode = cpm.ReturnCode;
                            // 処理結果コード移入
                            rResultCode = cpm.Result.Result_code;
                            // 確認のためアイアログ表示
                            string cpmres =
                                string.Format("ReturnCode:{0}", cpm.ReturnCode) + "\r\n" +
                                string.Format("ReturnMessage:{0}", cpm.ReturnMessage) + "\r\n" +
                                string.Format("MsgSummaryCode:{0}", cpm.MsgSummaryCode) + "\r\n" +
                                string.Format("MsgSummary:{0}", cpm.MsgSummary) + "\r\n" +
                                string.Format("Result.Partner_order_id:{0}", cpm.Result.Partner_order_id) + "\r\n" +
                                string.Format("Result.Currency:{0}", cpm.Result.Currency) + "\r\n" +
                                string.Format("Result.Order_id:{0}", cpm.Result.Order_id) + "\r\n" +
                                string.Format("Result.Return_code:{0}", cpm.Result.Return_code) + "\r\n" +
                                string.Format("Result.Result_code:{0}", cpm.Result.Result_code) + "\r\n" +
                                string.Format("Result.Create_time:{0}", cpm.Result.Create_time) + "\r\n" +
                                string.Format("Result.Total_fee:{0}", cpm.Result.Total_fee.ToString()) + "\r\n" +
                                string.Format("Result.Real_fee:{0}", cpm.Result.Real_fee.ToString()) + "\r\n" +
                                string.Format("Result.Channel:{0}", cpm.Result.Channel) + "\r\n" +
                                string.Format("Result.Pay_time:{0}", cpm.Result.Pay_time) + "\r\n" +
                                string.Format("Result.Order_body:{0}", cpm.Result.Order_body) + "\r\n" +
                                string.Format("BalanceAmount:{0}", cpm.BalanceAmount)
                                ;

                            Console.WriteLine(cpmres, "帰ってきたjsonパラメタ");

                        }
                        else if (mode == 1)
                        {
                            //   返金API使用時
                            reFoundR = new MakeJsons.ReFoundRes();
                            reFoundR = JsonConvert.DeserializeObject<MakeJsons.ReFoundRes>(g);
                            // 結果コード移入
                            rReturnCode = reFoundR.ReturnCode;
                            // 処理結果コード移入
                            rResultCode = reFoundR.Result.Result_code;
                            // 確認のためアイアログ表示
                            string cpmres =
                                string.Format("ReturnCode:{0}", reFoundR.ReturnCode) + "\r\n" +
                                string.Format("ReturnMessage:{0}", reFoundR.ReturnMessage) + "\r\n" +
                                string.Format("MsgSummaryCode:{0}", reFoundR.MsgSummaryCode) + "\r\n" +
                                string.Format("MsgSummary:{0}", reFoundR.MsgSummary) + "\r\n" +
                                string.Format("Result.Partner_refund_id:{0}", reFoundR.Result.Partner_refund_id) + "\r\n" +
                                string.Format("Result.Refund_id:{0}", reFoundR.Result.Refund_id) + "\r\n" +
                                string.Format("Result.Currency:{0}", reFoundR.Result.Currency) + "\r\n" +
                                string.Format("Result.Return_code:{0}", reFoundR.Result.Return_code) + "\r\n" +
                                string.Format("Result.Result_code:{0}", reFoundR.Result.Result_code) + "\r\n" +
                                string.Format("Result.Partner_order_id:{0}", reFoundR.Result.Partner_order_id) + "\r\n" +
                                string.Format("Result.Total_fee:{0}", reFoundR.Result.Amount.ToString()) + "\r\n" +
                                string.Format("Result.Channel:{0}", reFoundR.Result.Channel) + "\r\n" +
                                string.Format("Result.Order_id:{0}", reFoundR.Result.Order_id) + "\r\n" +
                                string.Format("Result.Create_time:{0}", reFoundR.Result.Create_time) + "\r\n" +
                                string.Format("Result.Pay_time:{0}", reFoundR.Result.Pay_time) + "\r\n" +
                                string.Format("Result.Total_fee:{0}", reFoundR.Result.Total_fee.ToString()) + "\r\n" +
                                string.Format("Result.Real_fee:{0}", reFoundR.Result.Real_fee.ToString())
                                ;

                            Console.WriteLine(cpmres, "帰ってきたjsonパラメタ");

                        }
                        // 処理正常でかつ支払完了時のみ支払確認処理をさせない
                        if ((rReturnCode == cReturnCode) && (rResultCode == cResult_Code_S))
                        {
                            rtn = true;
                            if (mode == 0)
                            {
                                MessageBox.Show("支払が完了しました。", "QRコード支払");
                            }
                            else if (mode == 1)
                            {
                                MessageBox.Show("返金処理が完了しました。", "返金処理");

                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Null had come...", "帰ってきたjsonパラメタ");
                    }
                }
                else
                {
                    Console.WriteLine("PUT失敗！");    
                    var g = res.StatusCode.ToString();
                    MessageBox.Show(g, "PUT失敗のStatusCode");
                }
                // http client開放
                client.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("あかんやん！: {0}", e.Message));
                Console.WriteLine(string.Format("なんでやねん！: {0}", e.InnerException));
                MessageBox.Show(string.Format("Exceptionが発生しましたよ。:\r\n{0} \r\ninner Ex:\r\n{1}", e.Message, e.InnerException),
                    "残念ながらPUTに失敗してます。");
            }

            return rtn;
        }

        #region "Http Header生成"
        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static DateTime UNIX_EPOCH =
          new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// httpヘッダーを編集する
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private void AddHttpHeader(ref HttpRequestMessage request)
        {
            // ミリ秒(3桁)単位をUNIX時間に変換(13桁)
            var unixTimeStamp = (long)DateTime.Now.Subtract(UNIX_EPOCH).TotalMilliseconds;
            request.Headers.Add("X-LAKALA-Time", unixTimeStamp.ToString());
            // ランダム文字列(15桁---半角英数字<英字は大小文字可>)
            string rndm = GetRandomWords(15);
            request.Headers.Add("X-LAKALA-NonceStr", rndm);
            /* 
             * トークンハッシュ
             * ログインID 、ミリ秒で表される現在の時間、ランダム文字列、 認証キー を
             * “＆”で 連結した文字列 の sha256 ハッシュ値 の 16 進数表記文字列 を小文字に変換する。
             */
            string uID = "abcdefg001";      // for TEST
            string uAUTH = "auth001OK002";  // for TEST
            
            string s = GetTalknHash(uID, unixTimeStamp.ToString(), rndm, uAUTH);
            request.Headers.Add("X-LAKALA-Sign", s);
            /*
             * ログインID
             * 発行した端末ユーザ ID（加盟店管理画面にて作成）
             * ユーザ認証APIのログインIDと同一
             */
            request.Headers.Add("X-LAKALA-loginId", uID);
            /*
             * 端末識別番号(半角英数記号 空白可 可変256---但し記号は「-」のみ)
             */
            string uSN = "KONAMISPORTSCLUB-SINAGAWAHONTEN-0001";
            request.Headers.Add("X-LAKALA-serialNo", uSN);
        }

        private void AddHttpHeader2(ref HttpClient client)
        {
            // ミリ秒(3桁)単位をUNIX時間に変換(13桁)
            var unixTimeStamp = (long)DateTime.Now.Subtract(UNIX_EPOCH).TotalMilliseconds;
            client.DefaultRequestHeaders.Add("X-LAKALA-Time", unixTimeStamp.ToString());
            // ランダム文字列(15桁---半角英数字<英字は大小文字可>)
            string rndm = GetRandomWords(15);
            client.DefaultRequestHeaders.Add("X-LAKALA-NonceStr", rndm);
            /* 
             * トークンハッシュ
             * ログインID 、ミリ秒で表される現在の時間、ランダム文字列、 認証キー を
             * “＆”で 連結した文字列 の sha256 ハッシュ値 の 16 進数表記文字列 を小文字に変換する。
             */
            string uID = "abcdefg001";      // for TEST
            string uAUTH = "auth001OK002";  // for TEST

            string s = GetTalknHash(uID, unixTimeStamp.ToString(), rndm, uAUTH);
            client.DefaultRequestHeaders.Add("X-LAKALA-Sign", s);
            /*
             * ログインID
             * 発行した端末ユーザ ID（加盟店管理画面にて作成）
             * ユーザ認証APIのログインIDと同一
             */
            client.DefaultRequestHeaders.Add("X-LAKALA-loginId", uID);
            /*
             * 端末識別番号(半角英数記号 空白可 可変256---但し記号は「-」のみ)
             */
            string uSN = "KONAMISPORTSCLUB-SINAGAWAHONTEN-0001";
            client.DefaultRequestHeaders.Add("X-LAKALA-serialNo", uSN);

        }

        /// <summary>
        /// ランダム文字列生成用対象文字列
        /// ここに記号を入れれば英数字(大小文字)＋記号も含めた中から指定文字数のランダム文字が生成される
        /// </summary>
        const string RNDM_CHARS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// ランダム文字列生成メソッド
        /// </summary>
        /// <param name="len">桁数</param>
        /// <param name="PW">対象文字列</param>
        /// <returns></returns>
        private string GetRandomWords(int len, string PW = RNDM_CHARS)
        {
            if (string.IsNullOrEmpty(PW)) PW = RNDM_CHARS;

            var r = new Random();
            return string.Join("", Enumerable.Range(0, len).Select(_ => RNDM_CHARS[r.Next(PW.Length)]));
        }
        /// <summary>
        /// 各文字列を「&」で連結した文字列の
        /// sha256ハッシュ値の16進数表記文字列を小文字に変換する
        /// </summary>
        /// <param name="uID">ユーザーID</param>
        /// <param name="uTM">現在時刻(ミリ秒)</param>
        /// <param name="uRNDM">ランダム文字列</param>
        /// <param name="uAKey">認証キー</param>
        /// <returns></returns>
        private string GetTalknHash(string uID, string uTM, string uRNDM, string uAKey)
        {
            string ret = string.Empty;
            const string amp = "&";
            // 文字列を「&」で結合
            var wSt = uID + amp + uTM + amp + uRNDM + amp + uAKey;
            // Unicodeエンコード
            byte[] vs = Encoding.UTF8.GetBytes(wSt);

            SHA256 hA256 = new SHA256CryptoServiceProvider();
            byte[] vs1 = hA256.ComputeHash(vs);
            ret = BitConverter.ToString(vs1).ToLower();
            Console.WriteLine(ret);
            return ret;
        }
        #endregion
        #region "送信用JSONデータ生成"
        /// <summary>
        /// QRコード支払(CPM)API[PUT METHOD]用 jsonデータ生成
        /// </summary>
        /// <param name="jdata">送信jsonデータ</param>
        /// <returns></returns>
        private bool PutCPMJson(ref string jdata,int mode = 0)
        {
            bool ret = true;
            try
            {
                if (mode == 0)
                {
                    // CPMリクエスト JSON定義
                    var req = new MakeJsons.CpmReq();
                    // Requestパラメタ設定
                    req.Order_id = "11234567890123456789";
                    req.SerialNo = "KONAMISPORTS CLUB HONTEN-4501001";
                    req.Description = "テストの取引備考でざます";
                    req.Price = 500;
                    req.Auth_code = "0001002003004005006007008009";
                    req.Currency = "JPY";
                    req.Operator = "abcdefg001";
                    // JSON シリアライズ(JSONフォーマットテキスト)
                    var JsonData = JsonConvert.SerializeObject(req);
                    jdata = JsonData;

                    Console.WriteLine("CPM PUT JSON:{0}", JsonData);
                }
                else if(mode == 1)
                {
                    // Refund(返金リクエスト)JSON定義クラス
                    var req = new MakeJsons.ReFoundReq();
                    // Requestパラメタ設定
                    req.Refund_id = "11234567892123456789";
                    req.Order_id = "98765432129876543211";
                    req.SerialNo = "SHINAGAWA HONTEN 4501001";
                    req.Fee = 12345678;
                    var JsonData = JsonConvert.SerializeObject(req);
                    jdata = JsonData;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        /// <summary>
        /// ユーザー認証API(PUT)用JSONデータ生成
        /// </summary>
        /// <param name="jdata">JSONデータ</param>
        /// <returns>true:成功、false:失敗</returns>
        private bool PostAuthJson(ref string jdata)
        {
            bool rtn = false;

            try
            {
                var auth = new MakeJsons.UserAuth();
                auth.LoginId = "SHINAGAWA_HONTEN_001";
                auth.UserPassword = "abCdeFgxYZ001";
                auth.OsName = "KONAMI SPORTS CASHLESS SYS";
                auth.OsVersion = "1.0.0.0.1";
                auth.SerialNo = "A-001-002-003";

                jdata = JsonConvert.SerializeObject(auth);
                rtn = true;
            }
            catch(Exception e)
            {
                Console.WriteLine("POST METHOD JSONデータ作成に失敗:{0}", e.Message);
            }

            return rtn;
        }
        #endregion

        /// <summary>
        /// GET METHOD PAULING
        /// 支払確認API実行
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetPauling(int mode=0)
        {
            // PAULING 間隔待機
            await Task.Delay(pGetPollTime);
            // 支払確認API実行
            return await GetApiFrmUrl(pUrl,mode);
            
        }

    }

}
