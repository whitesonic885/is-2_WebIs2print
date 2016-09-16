using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;

namespace is2print
{
	/// <summary>
	/// [is2print]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 東都）高木 オブジェクトの破棄
	//	disposeReader(reader);
	//	reader = null;
	// DEL 2007.05.10 東都）高木 未使用関数のコメント化
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害
	// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示
	//--------------------------------------------------------------------------
	// DEL 2008.06.12 kcl)森本 着店コード検索方法の変更に伴い、着店非表示機能を削除
	//  Get_InvoicePrintData
	// ADD 2008.07.09 東都）高木 未発行分を除外する 
	//--------------------------------------------------------------------------
	// ADD 2009.01.29 東都）高木 お届け先の一覧のソート順に[荷受人ＣＤ]を追加 
	// ADD 2009.01.29 東都）高木 お届け先の一覧のソート順に[名前２]を追加 
	// ADD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 
	// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 
	// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 
	//--------------------------------------------------------------------------
	// MOD 2010.02.03 東都）高木 検索条件に更新日を追加 
	// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 
	// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 
	//保留 MOD 2010.07.21 東都）高木 リコー様対応 
	// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 
	// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 
	//--------------------------------------------------------------------------
	// MOD 2011.01.06 東都）高木 郵便番号の印刷 
	// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない 
	// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 
	// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 
	// MOD 2011.04.13 東都）高木 重量入力不可対応 
	// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 
	// MOD 2011.07.14 東都）高木 記事行の追加 
	// MOD 2011.10.06 東都）高木 出荷データの印刷ログの追加 
	// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 
	//--------------------------------------------------------------------------
	// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加
	//--------------------------------------------------------------------------
	// MOD 2014.09.25 BEVAS）前田 支店止め対応
	//--------------------------------------------------------------------------
	// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2print")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		/*********************************************************************
		 * 出荷印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ
		 * 戻値：ステータス、荷受人ＣＤ、電話番号、住所...
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_InvoicePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷印刷データ取得開始");

// MOD 2005.06.08 東都）伊賀　指定日区分追加 START
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 START
//			string[] sRet = new string[34];
//			string[] sRet = new string[36];
			OracleConnection conn2 = null;
// MOD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 START
//			string[] sRet = new string[37];
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 START
//			string[] sRet = new string[39];
// MOD 2011.01.06 東都）高木 郵便番号の印刷 START
//			string[] sRet = new string[40];
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//			string[] sRet = new string[41];
// MOD 2011.07.14 東都）高木 記事行の追加 START
//			string[] sRet = new string[42];
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 START
//			string[] sRet = new string[45];
			string[] sRet = new string[46];
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 END
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.01.06 東都）高木 郵便番号の印刷 END
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 END
// MOD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 END
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 END
// MOD 2005.06.08 東都）伊賀　指定日区分追加 END
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
			string s利用者部門店所ＣＤ = (sKey.Length >  4) ? sKey[ 4] : "";
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

// ADD 2005.05.18 東都）小童谷 才数追加 START
			decimal d才数 = 0;
// ADD 2005.05.18 東都）小童谷 才数追加 END
// ADD 2005.06.09 東都）小童谷 郵便番号追加 START
			string s郵便番号 = "";
// ADD 2005.06.09 東都）小童谷 郵便番号追加 END
			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT ");
				sbQuery.Append(" ST01.荷受人ＣＤ ");
				sbQuery.Append(",ST01.電話番号１ ");
				sbQuery.Append(",ST01.電話番号２ ");
				sbQuery.Append(",ST01.電話番号３ ");
				sbQuery.Append(",ST01.住所１ ");
				sbQuery.Append(",ST01.住所２ ");
				sbQuery.Append(",ST01.住所３ ");
				sbQuery.Append(",ST01.名前１ ");
				sbQuery.Append(",ST01.名前２ ");
				sbQuery.Append(",ST01.出荷日 ");
				sbQuery.Append(",ST01.送り状番号 ");
				sbQuery.Append(",ST01.郵便番号 ");
				sbQuery.Append(",ST01.着店ＣＤ ");
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 START
//				sbQuery.Append(",ST01.発店ＣＤ ");
				sbQuery.Append(",NVL(CM14.店所ＣＤ, ST01.発店ＣＤ)");
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 END
				sbQuery.Append(",SM01.電話番号１ ");
				sbQuery.Append(",SM01.電話番号２ ");
				sbQuery.Append(",SM01.電話番号３ ");
				sbQuery.Append(",SM01.住所１ ");
				sbQuery.Append(",SM01.住所２ ");
				sbQuery.Append(",SM01.住所３ ");
				sbQuery.Append(",SM01.名前１ ");
				sbQuery.Append(",SM01.名前２ ");
				sbQuery.Append(",ST01.個数 ");
				sbQuery.Append(",ST01.重量 ");
				sbQuery.Append(",ST01.保険金額 ");
				sbQuery.Append(",ST01.指定日 ");
				sbQuery.Append(",ST01.輸送指示１ ");
				sbQuery.Append(",ST01.輸送指示２ ");
				sbQuery.Append(",ST01.品名記事１ ");
				sbQuery.Append(",ST01.品名記事２ ");
				sbQuery.Append(",ST01.品名記事３ ");
				sbQuery.Append(",ST01.元着区分 ");
				sbQuery.Append(",ST01.送り状発行済ＦＧ ");
// ADD 2005.05.18 東都）小童谷 才数追加 START
				sbQuery.Append(",ST01.才数 \n");
// ADD 2005.05.18 東都）小童谷 才数追加 END
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 START
				sbQuery.Append(",ST01.荷送人部署名 ");
				sbQuery.Append(",ST01.お客様出荷番号 ");
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 END
// ADD 2005.06.07 東都）伊賀 輸送商品コード個別対応追加 START
				sbQuery.Append(",ST01.輸送指示ＣＤ１ ");
				sbQuery.Append(",ST01.輸送指示ＣＤ２ ");
// ADD 2005.06.07 東都）伊賀 輸送商品コード個別対応追加 END
// ADD 2005.06.08 東都）伊賀 指定日区分追加 START
				sbQuery.Append(",ST01.指定日区分 ");
// ADD 2005.06.08 東都）伊賀 指定日区分追加 END
// ADD 2005.06.09 東都）小童谷 郵便番号追加 START
				sbQuery.Append(",ST01.郵便番号 ");
// ADD 2005.06.09 東都）小童谷 郵便番号追加 END
// ADD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 START
				sbQuery.Append(",ST01.仕分ＣＤ ");
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 START
//				sbQuery.Append(",ST01.発店名 ");
				sbQuery.Append(",NVL(CM10.店所名, ST01.発店名)");
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 END
// ADD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 END
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 START
				sbQuery.Append(",ST01.出荷済ＦＧ ");
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 END
// MOD 2011.01.06 東都）高木 郵便番号の印刷 START
				sbQuery.Append(",SM01.郵便番号 ");
// MOD 2011.01.06 東都）高木 郵便番号の印刷 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(",NVL(CM01.保留印刷ＦＧ,'0') \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
				sbQuery.Append(",ST01.品名記事４ ,ST01.品名記事５ ,ST01.品名記事６ \n");
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 START
				sbQuery.Append(",ST01.着店名 ");
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 END
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
				sbQuery.Append(",ST01.発店ＣＤ ");
				sbQuery.Append(",ST01.発店名 ");
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END
				sbQuery.Append(" FROM \"ＳＴ０１出荷ジャーナル\" ST01");
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 START
				sbQuery.Append("\n");
				sbQuery.Append(" LEFT JOIN ＣＭ０２部門 CM02 \n");
				sbQuery.Append(" ON ST01.会員ＣＤ = CM02.会員ＣＤ \n");
				sbQuery.Append("AND ST01.部門ＣＤ = CM02.部門ＣＤ \n");
				sbQuery.Append(" LEFT JOIN ＣＭ１４郵便番号 CM14 \n");
				sbQuery.Append(" ON CM02.郵便番号 = CM14.郵便番号 \n");
				sbQuery.Append(" LEFT JOIN ＣＭ１０店所 CM10 \n");
				sbQuery.Append(" ON CM14.店所ＣＤ = CM10.店所ＣＤ \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(" LEFT JOIN ＣＭ０１会員 CM01 \n");
				sbQuery.Append(" ON ST01.会員ＣＤ = CM01.会員ＣＤ \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 END
				sbQuery.Append(", \"ＳＭ０１荷送人\" SM01 \n");
				sbQuery.Append(" WHERE ST01.会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ST01.部門ＣＤ = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND ST01.登録日 = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND ST01.ジャーナルＮＯ = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND ST01.会員ＣＤ = SM01.会員ＣＤ \n");
				sbQuery.Append(" AND ST01.部門ＣＤ = SM01.部門ＣＤ \n");
				sbQuery.Append(" AND ST01.荷送人ＣＤ = SM01.荷送人ＣＤ \n");
				sbQuery.Append(" AND ST01.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND SM01.削除ＦＧ = '0' \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				int iCnt = 0;
				if (reader.Read())
				{
					string s輸送商品ＣＤ１ = reader.GetString(36).Trim();
					string s輸送商品ＣＤ２ = reader.GetString(37).Trim();
					sRet[1]  = reader.GetString(0).Trim();
					sRet[2]  = reader.GetString(1).Trim();
					sRet[3]  = reader.GetString(2).Trim();
					sRet[4]  = reader.GetString(3).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[5]  = reader.GetString(4).Trim();
//					sRet[6]  = reader.GetString(5).Trim();
//					sRet[7]  = reader.GetString(6).Trim();
//					sRet[8]  = reader.GetString(7).Trim();
//					sRet[9]  = reader.GetString(8).Trim();
					sRet[5]  = reader.GetString(4).TrimEnd(); // 荷受人住所１
					sRet[6]  = reader.GetString(5).TrimEnd(); // 荷受人住所２
					sRet[7]  = reader.GetString(6).TrimEnd(); // 荷受人住所３
					sRet[8]  = reader.GetString(7).TrimEnd(); // 荷受人名前１
					sRet[9]  = reader.GetString(8).TrimEnd(); // 荷受人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sRet[10] = reader.GetString(9).Trim();
					sRet[11] = reader.GetString(10).Trim();
					sRet[12] = reader.GetString(11).Trim();
					sRet[13] = reader.GetString(12).Trim().PadLeft(4, '0');
					sRet[14] = reader.GetString(13).Trim().PadLeft(4, '0');
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
					//社内伝の場合は、出荷テーブルの方を正とする
					if(s利用者部門店所ＣＤ == "044")
					{
						sRet[14] = reader.GetString(49).Trim().PadLeft(4, '0');
					}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END
					sRet[15] = reader.GetString(14).Trim();
					sRet[16] = reader.GetString(15).Trim();
					sRet[17] = reader.GetString(16).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[18] = reader.GetString(17).Trim();
//					sRet[19] = reader.GetString(18).Trim();
//					sRet[20] = reader.GetString(19).Trim();
//					sRet[21] = reader.GetString(20).Trim();
//					sRet[22] = reader.GetString(21).Trim();
					sRet[18] = reader.GetString(17).TrimEnd(); // 荷送人住所１
					sRet[19] = reader.GetString(18).TrimEnd(); // 荷送人住所２
					sRet[20] = reader.GetString(19).TrimEnd(); // 荷送人住所３
					sRet[21] = reader.GetString(20).TrimEnd(); // 荷送人名前１
					sRet[22] = reader.GetString(21).TrimEnd(); // 荷送人名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sRet[23] = reader.GetDecimal(22).ToString().Trim();
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//// MOD 2005.05.18 東都）小童谷 才数追加 START
////					sRet[24] = reader.GetDecimal(23).ToString().Trim();
//					d才数    = reader.GetDecimal(33);
//					d才数    = d才数 * 8;
//					if(d才数 == 0)
//						sRet[24] = reader.GetDecimal(23).ToString().Trim();
//					else
//						sRet[24] = d才数.ToString().Trim();
//// MOD 2005.05.18 東都）小童谷 才数追加 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					if(reader.GetString(44) == "1"){
						d才数 = reader.GetDecimal(33) * 8;
						if(d才数 == 0){
							sRet[24] = reader.GetDecimal(23).ToString().TrimEnd();
						}else{
							sRet[24] = d才数.ToString().TrimEnd();
						}
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						sRet[24] = "";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
					sRet[25] = reader.GetDecimal(24).ToString().Trim();
					sRet[26] = reader.GetString(25).Trim();
// MOD 2005.06.07 東都）伊賀 輸送商品コード個別対応追加 START
//					sRet[27] = reader.GetString(26).TrimEnd();
//					sRet[28] = reader.GetString(27).TrimEnd();
					// 時間指定の場合、２行目のみ表示
					if (s輸送商品ＣＤ１.Equals("100"))
					{
						sRet[27] = reader.GetString(27).TrimEnd();
						sRet[28] = "";
					}
					// １行目と２行目が同じコードの場合、２行目を表示しない
					else if (s輸送商品ＣＤ１.Equals(s輸送商品ＣＤ２))
					{
						sRet[27] = reader.GetString(26).TrimEnd();
						sRet[28] = "";
					}
					else
					{
						sRet[27] = reader.GetString(26).TrimEnd();
						sRet[28] = reader.GetString(27).TrimEnd();
					}
// MOD 2005.06.07 東都）伊賀 輸送商品コード個別対応追加 START
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[29] = reader.GetString(28).Trim();
//					sRet[30] = reader.GetString(29).Trim();
//					sRet[31] = reader.GetString(30).Trim();
					sRet[29] = reader.GetString(28).TrimEnd(); // 品名記事１
					sRet[30] = reader.GetString(29).TrimEnd(); // 品名記事２
					sRet[31] = reader.GetString(30).TrimEnd(); // 品名記事３
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
// MOD 2005.06.07 東都）伊賀 輸送商品によって値を変更 START
					// パーセルの場合、"11"
					if (s輸送商品ＣＤ１.Equals("001") || s輸送商品ＣＤ１.Equals("002"))
						sRet[32] = reader.GetString(31).Trim() + "1";
					else
						sRet[32] = reader.GetString(31).Trim() + "0";
// MOD 2005.06.07 東都）伊賀 輸送商品によって値を変更 END
					sRet[33] = reader.GetString(32).Trim(); // 送り状発行済ＦＧ
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 START
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sRet[34] = reader.GetString(34).Trim();
					sRet[34] = reader.GetString(34).TrimEnd(); // 担当者（部署）
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sRet[35] = reader.GetString(35).Trim(); // お客様番号
// ADD 2005.06.01 東都）小童谷 担当者、お客様番号追加 END
// ADD 2005.06.08 東都）伊賀 指定日区分追加 START
					sRet[36] = reader.GetString(38).Trim();
// ADD 2005.06.08 東都）伊賀 指定日区分追加 END
// ADD 2005.06.09 東都）小童谷 郵便番号追加 START
					s郵便番号 = reader.GetString(39).Trim();
// ADD 2005.06.09 東都）小童谷 郵便番号追加 END
// ADD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 START
					sRet[37] = reader.GetString(40).Trim();		//仕分ＣＤ
					sRet[38] = reader.GetString(41).Trim();		//発店名
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
					//社内伝の場合は、出荷テーブルの方を正とする
					if(s利用者部門店所ＣＤ == "044")
					{
						sRet[38] = reader.GetString(50).Trim();
					}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END
// ADD 2007.02.08 東都）高木 仕分ＣＤ、発店名の追加 END
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 START
					sRet[39] = reader.GetString(42).Trim();		//出荷済ＦＧ
// MOD 2010.11.01 東都）高木 出荷済の場合、出荷日未更新 END
// MOD 2011.01.06 東都）高木 郵便番号の印刷 START
					sRet[40] = reader.GetString(43).Trim();		//ご依頼主郵便番号
// MOD 2011.01.06 東都）高木 郵便番号の印刷 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					sRet[41] = reader.GetString(44).TrimEnd();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					sRet[42] = reader.GetString(45).TrimEnd(); // 品名記事４
					sRet[43] = reader.GetString(46).TrimEnd(); // 品名記事５
					sRet[44] = reader.GetString(47).TrimEnd(); // 品名記事６
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 START
					sRet[45] = reader.GetString(48).TrimEnd(); // 着店名
// MOD 2011.12.06 東都）高木 ラベルヘッダ部に発店名・着店名を印字 END
					iCnt++;
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (iCnt == 0)
				{
					sRet[0] = "該当データがありません";
				}
				else
				{
					sRet[0] = "正常終了";
// DEL 2008.06.12 kcl)森本 着店コード検索方法の変更 START
//// ADD 2005.06.09 東都）小童谷 着店非表示取得追加 START
//					string styaku = Get_tyakutennasi(sUser, conn2, s郵便番号);
//					if(styaku == "1")
//						sRet[13] = "";
//// ADD 2005.06.09 東都）小童谷 着店非表示取得追加 END
// DEL 2008.06.12 kcl)森本 着店コード検索方法の変更 END
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
					if(s利用者部門店所ＣＤ.Length == 0){
// MOD 2011.10.06 東都）高木 出荷データの印刷ログの追加 START
logWriter(sUser, INF, "出荷印刷データ取得　利用者部門店所ＣＤ無"
						+"["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sRet[11]+"]"
						+"送り状発行済["+sRet[33]+"]出荷済["+sRet[39]+"]"
						);
// MOD 2011.10.06 東都）高木 出荷データの印刷ログの追加 END
						return sRet;
					}
					// 利用者の部門の管轄店所ＣＤと登録者の発店ＣＤとが異なる場合
					string s発店ＣＤ = sRet[14].Trim().Substring(1, 3);
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
//					if(!s発店ＣＤ.Equals(s利用者部門店所ＣＤ))
//					{
//						return sRet;
//					}
					//社内伝の場合は、利用者の部門の管轄店所ＣＤと登録者の発店ＣＤとが異なってもよい
					if(s利用者部門店所ＣＤ != "044")
					{
						if(!s発店ＣＤ.Equals(s利用者部門店所ＣＤ))
						{
							return sRet;
						}
					}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END
					// 送り状番号がない場合には取得する
					if(sRet[11].Length == 0)
					{
						disconnect2(sUser, conn2);
						conn2 = null;
						string[] sRetInvoiceNo = Set_InvoiceNo2(sUser ,sKey, sRet, s利用者部門店所ＣＤ);
						if(sRetInvoiceNo[0].Length == 4){
//							sRet[11] = sRetInvoiceNo[1];
						}else{
							sRet[0] = sRetInvoiceNo[0];
						}
					}
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END
// MOD 2011.10.06 東都）高木 出荷データの印刷ログの追加 START
logWriter(sUser, INF, "出荷印刷データ取得"
						+"["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sRet[11]+"]"
						+"送り状発行済["+sRet[33]+"]出荷済["+sRet[39]+"]"
						);
// MOD 2011.10.06 東都）高木 出荷データの印刷ログの追加 END
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

// ADD 2005.06.09 東都）小童谷 着店非表示取得追加 START
		/*********************************************************************
		 * 着店非表示取得
		 * 引数：郵便番号
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		private string Get_tyakutennasi(string[] sUser, OracleConnection conn2, string sYuubin)
		{
			string sAri = "0";

			string cmdQuery
				= "SELECT 郵便番号 \n"
				+ " FROM ＣＭ１５着店非表示 \n"
				+ " WHERE 郵便番号 = '" + sYuubin + "' \n"
				+ "   AND 削除ＦＧ     = '0' \n";

			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

			bool bRead = reader.Read();
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
			if(bRead == true)
				sAri = "1";
			else
				sAri = "0";
			
			return sAri;
		}
// ADD 2005.06.09 東都）小童谷 着店非表示取得追加 END

		/*********************************************************************
		 * 採番の更新
		 * 引数：会員ＣＤ、部門ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_InvoiceNo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "採番更新開始");
			
			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			//トランザクションの設定
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				decimal i登録連番     = 0;
				decimal i開始原票番号 = 0;
				decimal i終了原票番号 = 0;
				decimal i最終原票番号 = 0;
				string  s割付日       = "";
				string  s有効期限     = "";
				string  s当日日付     = "";

				string cmdQuery_am12 = "SELECT";
				cmdQuery_am12 += " AM12.登録連番 ";
				cmdQuery_am12 += ",AM12.開始原票番号 ";
				cmdQuery_am12 += ",AM12.終了原票番号 ";
				cmdQuery_am12 += ",AM12.最終原票番号 ";
				cmdQuery_am12 += ",AM12.割付日 ";
				cmdQuery_am12 += ",AM12.有効期限 ";
				cmdQuery_am12 += ",TO_CHAR(SYSDATE,'YYYYMMDD') \n";
				cmdQuery_am12 += " FROM ＡＭ１２送り状採番 AM12 \n";
				cmdQuery_am12 += " WHERE AM12.会員ＣＤ = '" + sKey[0] + "' \n";
				cmdQuery_am12 += " AND AM12.部門ＣＤ = '" + sKey[1] + "' \n";
				cmdQuery_am12 += " AND AM12.元着区分 = '" + sKey[2] + "' \n";
				cmdQuery_am12 += " AND AM12.削除ＦＧ = '0' \n";
				cmdQuery_am12 += " FOR UPDATE \n";

				OracleDataReader reader_am12 = CmdSelect(sUser, conn2, cmdQuery_am12);
				int intCnt_am12 = 0;
				sRet[1] = "";
				if (reader_am12.Read())
				{
					i登録連番     = reader_am12.GetDecimal(0);
					i開始原票番号 = reader_am12.GetDecimal(1);
					i終了原票番号 = reader_am12.GetDecimal(2);
					i最終原票番号 = reader_am12.GetDecimal(3);
					s割付日       = reader_am12.GetString(4).Trim();
					s有効期限     = reader_am12.GetString(5).Trim();
					s当日日付     = reader_am12.GetString(6).Trim();
					intCnt_am12++;

					if (i最終原票番号 < i終了原票番号 && int.Parse(s有効期限) >= int.Parse(s当日日付))
					{
						//送り状番号のセット
						sRet[1] = (i最終原票番号 + 1).ToString();
					}
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader_am12);
				reader_am12 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (sRet[1].Length == 0)
				{
					//ＡＭ１２送り状採番にキーが存在しない、または
					//最終番号 >= 終了番号、または
					//有効期限 <  当日の時
					decimal i最大連番   = 0;
					decimal i開始番号   = 0;
					decimal i最終番号   = 0;
					decimal i終了番号   = 0;
					decimal i割付枚数   = 0;
					decimal i有効期限   = 0;
					decimal i有効期限年 = 0;
					decimal i有効期限月 = 0;
					decimal i有効期限日 = 0;

					//採番管理より新規原票番号枠を取得
					string cmdQuery_am10 = "SELECT";
					cmdQuery_am10 += " AM10.最大連番 ";
					cmdQuery_am10 += ",AM10.登録連番 ";
					cmdQuery_am10 += ",AM10.最終原票番号 ";
					cmdQuery_am10 += ",AM11.終了原票番号 ";
					cmdQuery_am10 += ",AM10.割付枚数 ";
					cmdQuery_am10 += ",AM10.有効期限 ";
					cmdQuery_am10 += ",TO_CHAR(SYSDATE,'YYYYMMDD') \n";
					cmdQuery_am10 += "FROM ＡＭ１０採番管理 AM10 ";
					cmdQuery_am10 += ",ＡＭ１１送り状番号 AM11 \n";
					cmdQuery_am10 += " WHERE AM10.採番区分 = '" + sKey[2] + "' \n";
					//cmdQuery_am10 += "   AND AM10.登録連番       =  " + i登録連番;
					cmdQuery_am10 += " AND AM10.採番区分 = AM11.元着区分 \n";
					cmdQuery_am10 += " AND AM10.登録連番 = AM11.登録連番 \n";
					cmdQuery_am10 += " AND AM10.削除ＦＧ = '0' \n";
					cmdQuery_am10 += " FOR UPDATE \n";

					OracleDataReader reader_am10 = CmdSelect(sUser, conn2, cmdQuery_am10);
					int intCnt_am10 = 0;
					if (reader_am10.Read())
					{
						i最大連番     = reader_am10.GetDecimal(0);
						i登録連番     = reader_am10.GetDecimal(1);
						i最終番号     = reader_am10.GetDecimal(2);
						i終了番号     = reader_am10.GetDecimal(3);
						i割付枚数     = reader_am10.GetDecimal(4);
						i有効期限     = reader_am10.GetDecimal(5);
						s当日日付     = reader_am10.GetString(6);

						//送り状採番更新情報の取得
						i開始原票番号 = i最終番号 + 1;
						i終了原票番号 = i最終番号 + i割付枚数;
						i最終原票番号 = i開始原票番号;
						s割付日       = s当日日付;
						i有効期限年   = int.Parse(s割付日.Substring(0, 4));
						i有効期限月   = int.Parse(s割付日.Substring(4, 2)) + i有効期限 - 1;
						if (i有効期限月 > 12)
						{
							i有効期限年++;
							i有効期限月 = i有効期限月 - 12;
						}
						i有効期限日   = System.DateTime.DaysInMonth(decimal.ToInt32(i有効期限年), decimal.ToInt32(i有効期限月));
						s有効期限     = i有効期限年.ToString() + i有効期限月.ToString().PadLeft(2, '0') + i有効期限日.ToString().PadLeft(2, '0');

						//採番管理更新情報の取得
						i最終番号     = i終了原票番号;

						sRet[1] = i最終原票番号.ToString();
						intCnt_am10++;
					}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
					disposeReader(reader_am10);
					reader_am10 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
					if (intCnt_am10 == 0)
					{
						//該当データがない場合はエラー
						throw new Exception("該当データがありません");
					}
					if (i最終番号 > i終了番号)
					{
						i登録連番++;
						if (i登録連番 > i最大連番)
						{
							i登録連番 = 1;
						}
						//送り状番号より新規原票番号枠を取得
						string cmdQuery_am11 = "SELECT";
						cmdQuery_am11 += " AM11.開始原票番号 \n";
						cmdQuery_am11 += " FROM ＡＭ１１送り状番号 AM11 \n";
						cmdQuery_am11 += " WHERE AM11.元着区分 = '" + sKey[2] + "' \n";
						cmdQuery_am11 += " AND AM11.登録連番 =  " + i登録連番 + " \n";
						cmdQuery_am11 += " AND AM11.削除ＦＧ = '0' \n";
						cmdQuery_am11 += " FOR UPDATE \n";

						OracleDataReader reader_am11 = CmdSelect(sUser, conn2, cmdQuery_am11);
						int intCnt_am11 = 0;
						if (reader_am11.Read())
						{
							i開始番号     = reader_am11.GetDecimal(0);
							//採番管理更新情報の取得
							i最終番号     = i開始番号 + i割付枚数 - 1;
							//送り状採番更新情報の取得
							i開始原票番号 = i開始番号;
							i終了原票番号 = i最終番号;
							i最終原票番号 = i開始原票番号;

							sRet[1] = i最終原票番号.ToString();
							intCnt_am11++;
						}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
						disposeReader(reader_am11);
						reader_am11 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
						if (intCnt_am11 == 0)
						{
							//該当データがない場合はエラー
							throw new Exception("該当データがありません");
						}
					}
					// 採番管理の更新
					string updQuery_am10 = "UPDATE ＡＭ１０採番管理 \n";
					updQuery_am10 += " SET 登録連番 = " + i登録連番;
					updQuery_am10 += ", 最終原票番号 = " + i最終番号;
					updQuery_am10 += ", 更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "; // 更新日時
					updQuery_am10 += ", 更新者 = '" + sKey[3] + "' \n";                   // 更新者
					updQuery_am10 += " WHERE 採番区分 = '" + sKey[2] + "' \n";

					CmdUpdate(sUser, conn2, updQuery_am10);
				}

				string updQuery_am12 = "";
				if (intCnt_am12 == 0)
				{
					// 送り状採番の追加
					updQuery_am12  = "INSERT INTO ＡＭ１２送り状採番 \n";
					updQuery_am12 += " VALUES ('" + sKey[0] + "' ";
					updQuery_am12 +=         ",'" + sKey[1] + "' ";
					updQuery_am12 +=         ",'" + sKey[2] + "' ";
					updQuery_am12 +=         ", " + i登録連番;
					updQuery_am12 +=         ", " + i開始原票番号;
					updQuery_am12 +=         ", " + i終了原票番号;
					updQuery_am12 +=         ", " + i最終原票番号;
					updQuery_am12 +=         ",'" + s割付日 + "' ";
					updQuery_am12 +=         ",'" + s有効期限 + "' ";
					updQuery_am12 +=         ",'0' ";
					updQuery_am12 +=         ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=         ",'出荷登録' ";
					updQuery_am12 +=         ",'" + sKey[3] + "' ";
					updQuery_am12 +=         ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=         ",'出荷登録' ";
					updQuery_am12 +=         ",'" + sKey[3] + "' ";
					updQuery_am12 += " ) ";
				}
				else
				{
					// 送り状採番の更新
					updQuery_am12  = "UPDATE ＡＭ１２送り状採番 \n";
					updQuery_am12 += " SET 登録連番 =  " + i登録連番;
					updQuery_am12 +=      ", 開始原票番号 =  " + i開始原票番号;
					updQuery_am12 +=      ", 終了原票番号 =  " + i終了原票番号;
					updQuery_am12 +=      ", 最終原票番号 =  " + sRet[1];
					updQuery_am12 +=      ", 割付日 = '" + s割付日 + "'";
					updQuery_am12 +=      ", 有効期限 = '" + s有効期限 + "'";
					updQuery_am12 +=      ", 更新日時 =   TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=      ", 更新ＰＧ = '出荷登録' ";
					updQuery_am12 +=      ", 更新者 = '" + sKey[3] + "' \n";
					updQuery_am12 += " WHERE 会員ＣＤ = '" + sKey[0] + "' \n";
					updQuery_am12 +=   " AND 部門ＣＤ = '" + sKey[1] + "' \n";
					updQuery_am12 +=   " AND 元着区分 = '" + sKey[2] + "' \n";
				}
				CmdUpdate(sUser, conn2, updQuery_am12);
				tran.Commit();
				sRet[0] = "正常終了";
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * 送り状発行済ＦＧの更新
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ、送り状番号、更新者
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Set_InvoiceNo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "発行済ＦＧ更新開始");

			OracleConnection conn2 = null;
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
//			string[] sRet = new string[1];
			string[] sRet = new string[2]{"",""};
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 START
				StringBuilder sbQuery = new StringBuilder(1024);
				string s発店ＣＤ = "";
				string s発店名   = "";
				sbQuery.Append("SELECT NVL(CM14.店所ＣＤ, ' ') \n");
				sbQuery.Append(", NVL(CM10.店所名, ' ') \n");
				sbQuery.Append(" FROM ＣＭ０２部門 CM02 \n");
				sbQuery.Append(" LEFT JOIN ＣＭ１４郵便番号 CM14 \n");
				sbQuery.Append(" ON CM02.郵便番号 = CM14.郵便番号 \n");
				sbQuery.Append(" LEFT JOIN ＣＭ１０店所 CM10 \n");
				sbQuery.Append(" ON CM14.店所ＣＤ = CM10.店所ＣＤ \n");
				sbQuery.Append(" WHERE CM02.会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND CM02.部門ＣＤ = '" + sKey[1] + "' \n");
				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s発店ＣＤ = reader.GetString(0).Trim();
					s発店名   = reader.GetString(1).Trim();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 END
//保留 MOD 2010.07.21 東都）高木 リコー様対応 START
//				if(s発店ＣＤ == "030"){
//					sbQuery = new StringBuilder(1024);
//					sbQuery.Append("SELECT NVL(CM14.店所ＣＤ, ' ') \n");
//					sbQuery.Append(", NVL(CM10.店所名, ' ') \n");
//					sbQuery.Append(" FROM \"ＳＴ０１出荷ジャーナル\" ST01 \n");
//					sbQuery.Append(" LEFT JOIN ＳＭ０１荷送人 SM01 \n");
//					sbQuery.Append(" ON  ST01.会員ＣＤ = SM01.会員ＣＤ \n");
//					sbQuery.Append(" AND ST01.部門ＣＤ = SM01.部門ＣＤ \n");
//					sbQuery.Append(" AND ST01.荷送人ＣＤ = SM01.荷送人ＣＤ \n");
//					sbQuery.Append(" LEFT JOIN ＣＭ１４郵便番号 CM14 \n");
//					sbQuery.Append(" ON SM01.郵便番号 = CM14.郵便番号 \n");
//					sbQuery.Append(" LEFT JOIN ＣＭ１０店所 CM10 \n");
//					sbQuery.Append(" ON CM14.店所ＣＤ = CM10.店所ＣＤ \n");
//					sbQuery.Append(" WHERE ST01.会員ＣＤ = '" + sKey[0] + "' \n");
//					sbQuery.Append(" AND ST01.部門ＣＤ = '" + sKey[1] + "' \n");
//					sbQuery.Append(" AND ST01.登録日 = '" + sKey[2] + "' \n");
//					sbQuery.Append(" AND ST01.ジャーナルＮＯ = '" + sKey[3] + "' \n");
//					reader = CmdSelect(sUser, conn2, sbQuery);
//					if(reader.Read()){
//						s発店ＣＤ = reader.GetString(0).Trim();
//						s発店名   = reader.GetString(1).Trim();
//					}
//					disposeReader(reader);
//					reader = null;
//					sbQuery = null;
//				}
//保留 MOD 2010.07.21 東都）高木 リコー様対応 END
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
				// 送り状番号チェック
				sbQuery = new StringBuilder(1024);
				string s送り状番号 = "";
				sbQuery.Append("SELECT 送り状番号 \n");
				sbQuery.Append(" FROM  \"ＳＴ０１出荷ジャーナル\" \n");
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND 部門ＣＤ = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND 登録日   = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND \"ジャーナルＮＯ\" = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND 削除ＦＧ = '0' \n");
				sbQuery.Append(" FOR UPDATE \n");
				reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s送り状番号 = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
				if(s送り状番号.Length > 0){
					// 異なる送り状番号を上書きしようとした場合
					if(s送り状番号 != sKey[4]){
						tran.Commit();
						sRet[0] = "エラー：他の端末で印刷中もしくは印刷済です\n"
								+ "["+s送り状番号.Substring(4)+"]";
						sRet[1] = s送り状番号;
logWriter(sUser, INF, "送り状番号更新済["+sRet[1]+"]"
						+ " ["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sKey[4]+"]");
						return sRet;
					}
				}

// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END
				// 出荷ジャーナルの更新
				string cmdQuery  = "UPDATE \"ＳＴ０１出荷ジャーナル\" \n";
				cmdQuery += " SET 送り状番号 = '"  + sKey[4] + "' ";                     // 送り状番号
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
				cmdQuery +=     ",処理０１ = TO_CHAR(SYSDATE,'MMDDHH24MISS') \n"; // 送り状印刷月日時分秒
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END
				cmdQuery +=     ",送り状発行済ＦＧ = '1' ";
				cmdQuery +=     ",状態 = DECODE(状態,'01','02','02','02',状態) ";
				cmdQuery +=     ",詳細状態 = DECODE(状態,'01','  ','02','  ',詳細状態) ";
				cmdQuery +=     ",更新日時 =   TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";    // 更新日時
				cmdQuery +=     ",更新ＰＧ = '出荷登録' ";                               // 更新ＰＧ
				cmdQuery +=     ",更新者 = '" + sKey[5] + "' \n";                        // 更新者
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 START
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
//				if(s発店ＣＤ.Length > 0)
//				{
//					cmdQuery += ",発店ＣＤ = '" + s発店ＣＤ + "' \n";
//				}
//				if(s発店名.Length > 0){
//					cmdQuery += ",発店名 = '"   + s発店名   + "' \n";
//				}
				//社内伝の場合は、出荷テーブルの方を正とする
				if(sKey[0].Substring(0,2).ToUpper() != "FK")
				{
					if(s発店ＣＤ.Length > 0)
					{
						cmdQuery += ",発店ＣＤ = '" + s発店ＣＤ + "' \n";
					}
					if(s発店名.Length > 0)
					{
						cmdQuery += ",発店名 = '"   + s発店名   + "' \n";
					}
				}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END
// MOD 2010.06.03 東都）高木 郵便番号マスタの店所変更時の対応 END
				cmdQuery += " WHERE 会員ＣＤ       = '" + sKey[0] + "' \n";
				cmdQuery +=   " AND 部門ＣＤ       = '" + sKey[1] + "' \n";
				cmdQuery +=   " AND 登録日         = '" + sKey[2] + "' \n";
				cmdQuery +=   " AND ジャーナルＮＯ = '" + sKey[3] + "' \n";
				cmdQuery +=   " AND 削除ＦＧ       = '0' \n";
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 START
logWriter(sUser, INF, "発行済ＦＧ更新["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sKey[4]+"]");
// MOD 2010.06.18 東都）高木 出荷データの参照・追加・更新・削除ログの追加 END

				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "正常終了";
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 START
		/*********************************************************************
		 * 送り状番号更新
		 * 引数：会員ＣＤ、部門ＣＤ、登録日、ジャーナルＮＯ、送り状番号、更新者
		 * 　　　印刷データ、利用者部門店所ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
//		[WebMethod]
		private String[] Set_InvoiceNo2(string[] sUser, string[] sKey, string[] sPrintData, string sTensyo)
		{
			logWriter(sUser, INF, "送り状番号更新２開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]{"",""};

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try{
				StringBuilder sbQuery = new StringBuilder(1024);
				string s送り状番号 = "";
				sbQuery.Append("SELECT 送り状番号 \n");
				sbQuery.Append(" FROM  \"ＳＴ０１出荷ジャーナル\" \n");
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND 部門ＣＤ = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND 登録日   = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND \"ジャーナルＮＯ\" = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND 削除ＦＧ = '0' \n");
				sbQuery.Append(" FOR UPDATE \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s送り状番号 = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
				if(s送り状番号.Length > 0){
					tran.Commit();
					sRet[0] = "採番済み";
					sRet[1] = s送り状番号;
logWriter(sUser, INF, "送り状番号更新２　送り状番号更新済["+s送り状番号+"]");
					return sRet;
				}
				// 送り状番号チェック
				String[] sGetKey = new string[4];
//				sGetKey[1] = gs会員ＣＤ;
//				sGetKey[1] = gs部門ＣＤ;
//				sGetKey[0] = sUser[0];
				sGetKey[0] = sKey[0];
//				sGetKey[1] = sKey[1];
				sGetKey[1] = sTensyo; // 利用者部門店所ＣＤ
				sGetKey[2] = sPrintData[32]; //元着区分 + "0" or "1"
				if(sPrintData[14].Substring(1, 3) == "047"){
					sGetKey[2] = sPrintData[32].Substring(0,1) + "G"; //元着区分 + "G"
				}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 START
				if(sTensyo == "044")
				{
					sGetKey[2] = sPrintData[32].Substring(0,1) + "F"; //元着区分 + "F"
				}
// MOD 2016.04.15 bevas) 松本 社内伝票機能追加対応 END

//				sGetKey[3] = gs利用者ＣＤ;
				sGetKey[3] = sUser[1];
				String[] sGetData = this.Get_InvoiceNo(sUser, sGetKey);
				if(sGetData[0].Length != 4){
					tran.Commit();
					sRet[0] = sGetData[0];
					return sRet;
				}
				//送り状番号のセット
				sPrintData[11] = sGetData[1].PadLeft(14, '0');
				//チェックデジット（７で割った余り）の付加
				sPrintData[11] = sPrintData[11] + (long.Parse(sPrintData[11]) % 7).ToString();

				// 出荷ジャーナルの更新
				string cmdQuery  = "UPDATE \"ＳＴ０１出荷ジャーナル\" \n";
				cmdQuery += " SET 送り状番号 = '"  + sPrintData[11] + "' ";                     // 送り状番号
				cmdQuery += " WHERE 会員ＣＤ       = '" + sKey[0] + "' \n";
				cmdQuery +=   " AND 部門ＣＤ       = '" + sKey[1] + "' \n";
				cmdQuery +=   " AND 登録日         = '" + sKey[2] + "' \n";
				cmdQuery +=   " AND ジャーナルＮＯ = '" + sKey[3] + "' \n";
				cmdQuery +=   " AND 削除ＦＧ       = '0' \n";

				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "正常終了";
//				sRet[1] = sPrintData[11];
logWriter(sUser, INF, "送り状番号更新２　送り状番号更新["+sPrintData[11]+"]");
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2011.03.25 東都）高木 送り状番号の上書き防止 END

		/*********************************************************************
		 * 依頼主印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、荷送人ＣＤ、カナ略称、電話番号...
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ConsignorPrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "依頼主印刷データ取得開始");
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 START
			string s会員ＣＤ     = (sKey.Length >  0) ? sKey[ 0] : "";
			string s部門ＣＤ     = (sKey.Length >  1) ? sKey[ 1] : "";
			string s依頼主カナ   = (sKey.Length >  2) ? sKey[ 2] : "";
			if(s依頼主カナ == null) s依頼主カナ = "";
			string s依頼主コード = (sKey.Length >  3) ? sKey[ 3] : "";
			string s依頼主名前   = (sKey.Length >  4) ? sKey[ 4] : "";
			string s請求先ＣＤ   = (sKey.Length >  5) ? sKey[ 5] : "";
			string s部課ＣＤ     = (sKey.Length >  6) ? sKey[ 6] : "";
			string s階層リスト１ = (sKey.Length >  7) ? sKey[ 7] : "2";
			string sソート方向１ = (sKey.Length >  8) ? sKey[ 8] : "0";
			string s階層リスト２ = (sKey.Length >  9) ? sKey[ 9] : "0";
			string sソート方向２ = (sKey.Length > 10) ? sKey[10] : "0";

			int i階層リスト１ = 2;
			int iソート方向１ = 0;
			int i階層リスト２ = 0;
			int iソート方向２ = 0;
			try{
				i階層リスト１ = int.Parse(s階層リスト１);
				iソート方向１ = int.Parse(sソート方向１);
				i階層リスト２ = int.Parse(s階層リスト２);
				iソート方向２ = int.Parse(sソート方向２);
			}catch(Exception){
				;
			}
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 END

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT");
				sbQuery.Append(" SM01.荷送人ＣＤ ");
				sbQuery.Append(",SM01.カナ略称 ");
				sbQuery.Append(",SM01.電話番号１ ");
				sbQuery.Append(",SM01.電話番号２ ");
				sbQuery.Append(",SM01.電話番号３ ");
				sbQuery.Append(",SM01.郵便番号 ");
				sbQuery.Append(",SM01.住所１ ");
				sbQuery.Append(",SM01.住所２ ");
				sbQuery.Append(",SM01.住所３ ");
				sbQuery.Append(",SM01.名前１ ");
				sbQuery.Append(",SM01.名前２ ");
				sbQuery.Append(",SM01.重量 ");
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 START
//				sbQuery.Append(",NVL(SM04.得意先ＣＤ, ' ') ");
//				sbQuery.Append(",NVL(SM04.得意先部課ＣＤ, ' ') ");
				sbQuery.Append(",NVL(SM01.得意先ＣＤ, ' ') ");
				sbQuery.Append(",NVL(SM01.得意先部課ＣＤ, ' ') ");
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 END
				sbQuery.Append(",NVL(SM04.得意先部課名, ' ') \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(",SM01.才数 \n");
				sbQuery.Append(",NVL(CM01.保留印刷ＦＧ,'0') \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				sbQuery.Append(" FROM \"ＳＭ０１荷送人\" SM01 \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(" LEFT JOIN ＣＭ０１会員 CM01 \n");
				sbQuery.Append(" ON SM01.会員ＣＤ = CM01.会員ＣＤ \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
				sbQuery.Append(" LEFT JOIN \"ＣＭ０２部門\" CM02 \n");
				sbQuery.Append(" ON SM01.会員ＣＤ = CM02.会員ＣＤ ");
				sbQuery.Append("AND SM01.部門ＣＤ = CM02.部門ＣＤ ");
				sbQuery.Append("AND CM02.削除ＦＧ = '0' \n");
				sbQuery.Append(" LEFT JOIN \"ＳＭ０４請求先\" SM04 \n");
				sbQuery.Append(" ON CM02.郵便番号 = SM04.郵便番号 ");
				sbQuery.Append("AND SM01.得意先ＣＤ = SM04.得意先ＣＤ ");
				sbQuery.Append("AND SM01.得意先部課ＣＤ = SM04.得意先部課ＣＤ ");
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
				sbQuery.Append("AND SM01.会員ＣＤ = SM04.会員ＣＤ ");
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
				sbQuery.Append("AND SM04.削除ＦＧ = '0' \n");
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 START
//				sbQuery.Append(" WHERE SM01.会員ＣＤ = '" + sKey[0] + "' \n");
//				sbQuery.Append(" AND SM01.部門ＣＤ = '" + sKey[1] + "' \n");
//				sbQuery.Append(" AND SM01.削除ＦＧ = '0' \n");
//				sbQuery.Append(" ORDER BY SM01.荷送人ＣＤ \n");
				sbQuery.Append(" WHERE SM01.会員ＣＤ = '" + s会員ＣＤ + "' \n");
				sbQuery.Append(" AND SM01.部門ＣＤ = '" + s部門ＣＤ + "' \n");

				if(s依頼主カナ.Length > 0){
					sbQuery.Append(" AND SM01.カナ略称 LIKE '"+ s依頼主カナ + "%' \n");
				}
				if(s依頼主コード.Length > 0){
					sbQuery.Append(" AND SM01.荷送人ＣＤ LIKE '"+ s依頼主コード + "%' \n");
				}
				if(s依頼主名前.Length > 0){
					sbQuery.Append(" AND SM01.名前１ LIKE '%"+ s依頼主名前 + "%' \n");
				}
				if(s請求先ＣＤ.Length > 0){
					sbQuery.Append(" AND SM01.得意先ＣＤ = '"+ s請求先ＣＤ + "' \n");
					if(s部課ＣＤ.Length > 0){
						sbQuery.Append(" AND SM01.得意先部課ＣＤ = '"+ s部課ＣＤ + "' \n");
					}else{
						sbQuery.Append(" AND SM01.得意先部課ＣＤ = ' ' \n");
					}
				}
				sbQuery.Append(  "AND SM01.削除ＦＧ = '0' \n");

				sbQuery.Append(" ORDER BY \n");
				switch(i階層リスト１){
				case 1:
					sbQuery.Append(" SM01.カナ略称 ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 2:
					sbQuery.Append(" SM01.荷送人ＣＤ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 3:
//					sbQuery.Append(" SM01.得意先ＣＤ ");
//					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.得意先部課ＣＤ ");
					sbQuery.Append(" NVL(SM04.得意先部課名,SM01.得意先ＣＤ || SM01.得意先部課ＣＤ) ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 4:
					sbQuery.Append(" SM01.名前１ ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.名前２ ");
//					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 5:
					sbQuery.Append(" SM01.電話番号１ ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.電話番号２ ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.電話番号３ ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 6:
					sbQuery.Append(" SM01.登録日時 ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				case 7:
					sbQuery.Append(" SM01.更新日時 ");
					if(iソート方向１ == 1) sbQuery.Append(" DESC ");
					break;
				}
				if(i階層リスト１ != 0 && i階層リスト２ != 0){
					sbQuery.Append(",");
				}
				switch(i階層リスト２){
				case 1:
					sbQuery.Append(" SM01.カナ略称 ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 2:
					sbQuery.Append(" SM01.荷送人ＣＤ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 3:
//					sbQuery.Append(" SM01.得意先ＣＤ ");
//					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.得意先部課ＣＤ ");
					sbQuery.Append(" NVL(SM04.得意先部課名,SM01.得意先ＣＤ || SM01.得意先部課ＣＤ) ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 4:
					sbQuery.Append(" SM01.名前１ ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.名前２ ");
//					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 5:
					sbQuery.Append(" SM01.電話番号１ ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.電話番号２ ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.電話番号３ ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 6:
					sbQuery.Append(" SM01.登録日時 ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				case 7:
					sbQuery.Append(" SM01.更新日時 ");
					if(iソート方向２ == 1) sbQuery.Append(" DESC ");
					break;
				}
				if(i階層リスト１ == 0 && i階層リスト２ == 0){
					sbQuery.Append(" SM01.名前１ \n");
				}
				if(i階層リスト１ != 2 && i階層リスト２ != 2){
					sbQuery.Append(", SM01.荷送人ＣＤ \n");
				}
// MOD 2010.09.08 東都）高木 ＣＳＶ出力機能の追加 END

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//					string[] sData = new string[16];
					string[] sData = new string[18];
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // カナ略称
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
// ADD 2005.09.02 東都）小童谷 Trimはずし START
//					sData[5]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(0,3);
//					sData[6]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(3,4);
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// ADD 2005.09.02 東都）小童谷 Trimはずし START
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); // 住所１
					sData[8]  = reader.GetString(7).TrimEnd(); // 住所２
					sData[9]  = reader.GetString(8).TrimEnd(); // 住所３
					sData[10] = reader.GetString(9).TrimEnd(); // 名前１
					sData[11] = reader.GetString(10).TrimEnd();// 名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[12] = reader.GetDecimal(11).ToString().Trim();
					sData[13] = reader.GetString(12).Trim();
					sData[14] = reader.GetString(13).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[15] = reader.GetString(14).Trim();
					sData[15] = reader.GetString(14).TrimEnd(); // 得意先部課名
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					sData[16] = reader.GetDecimal(15).ToString().TrimEnd(); // 才数
					sData[17] = reader.GetString(16).TrimEnd(); // 重量入力制御
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * 届け先印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、荷受人ＣＤ、カナ略称、電話番号...
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ConsigneePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "届け先印刷データ取得開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT");
				sbQuery.Append(" 荷受人ＣＤ ");
				sbQuery.Append(",カナ略称 ");
				sbQuery.Append(",電話番号１ ");
				sbQuery.Append(",電話番号２ ");
				sbQuery.Append(",電話番号３ ");
				sbQuery.Append(",郵便番号 ");
				sbQuery.Append(",住所１ ");
				sbQuery.Append(",住所２ ");
				sbQuery.Append(",住所３ ");
				sbQuery.Append(",名前１ ");
				sbQuery.Append(",名前２ ");
				sbQuery.Append(",特殊計 \n");
				sbQuery.Append(" FROM \"ＳＭ０２荷受人\" \n");
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND 部門ＣＤ = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND 削除ＦＧ = '0' \n");
				sbQuery.Append(" ORDER BY 荷受人ＣＤ \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
					string[] sData = new string[13];
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // カナ略称
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
// ADD 2005.09.02 東都）小童谷 Trimはずし START
//					sData[5]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(0,3);
//					sData[6]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(3,4);
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// ADD 2005.09.02 東都）小童谷 Trimはずし START
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); // 住所１
					sData[8]  = reader.GetString(7).TrimEnd(); // 住所２
					sData[9]  = reader.GetString(8).TrimEnd(); // 住所３
					sData[10] = reader.GetString(9).TrimEnd(); // 名前１
					sData[11] = reader.GetString(10).TrimEnd();// 名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[12] = reader.GetString(11).Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * 記事印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、記事ＣＤ、記事
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_NotePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "記事印刷データ取得開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			try
			{
				//string cmdQuery = "SELECT";
				//cmdQuery += " 記事ＣＤ ";
				//cmdQuery += ",記事 ";
				//cmdQuery +=  "FROM \"ＳＭ０３記事\" ";
				//cmdQuery += "WHERE 会員ＣＤ   = '" + sKey[0] + "' ";
				//cmdQuery +=   "AND 部門ＣＤ   = '" + sKey[1] + "' ";
				//cmdQuery +=   "AND 削除ＦＧ   = '0' ";
				//cmdQuery += "ORDER BY 記事ＣＤ \n";
				//輸送指示の取得
				System.Text.StringBuilder cmdQuery_y = new System.Text.StringBuilder(256);
				cmdQuery_y.Append("SELECT ");
				cmdQuery_y.Append(" SM03_1.記事ＣＤ ");
				cmdQuery_y.Append(",SM03_1.記事 ");
				cmdQuery_y.Append(",NVL(SM03_2.記事ＣＤ, ' ') ");
				cmdQuery_y.Append(",NVL(SM03_2.記事, ' ') ");
				cmdQuery_y.Append(" FROM \"ＳＭ０３記事\" SM03_1 ");
				cmdQuery_y.Append(" LEFT JOIN \"ＳＭ０３記事\" SM03_2 ");
				cmdQuery_y.Append(       " ON SM03_1.会員ＣＤ = SM03_2.会員ＣＤ ");
				cmdQuery_y.Append(      " AND SM03_1.記事ＣＤ = SM03_2.部門ＣＤ ");
				cmdQuery_y.Append(      " AND '0'             = SM03_2.削除ＦＧ ");
				cmdQuery_y.Append("WHERE SM03_1.会員ＣＤ   = 'yusoshohin' ");
				cmdQuery_y.Append(  "AND SM03_1.部門ＣＤ   = '0000' ");
				cmdQuery_y.Append(  "AND SM03_1.削除ＦＧ   = '0' ");
				cmdQuery_y.Append("ORDER BY SM03_1.記事ＣＤ,SM03_2.記事ＣＤ \n");
				OracleDataReader reader_y = CmdSelect(sUser, conn2, cmdQuery_y);

				//品名記事の取得
				System.Text.StringBuilder cmdQuery_h = new System.Text.StringBuilder(256);
				cmdQuery_h.Append("SELECT ");
				cmdQuery_h.Append(" 記事ＣＤ ");
				cmdQuery_h.Append(",記事 ");
				cmdQuery_h.Append(" FROM \"ＳＭ０３記事\" ");
				cmdQuery_h.Append("WHERE 会員ＣＤ   = '" + sKey[0] + "' ");
				cmdQuery_h.Append(  "AND 部門ＣＤ   = '" + sKey[1] + "' ");
				cmdQuery_h.Append(  "AND 削除ＦＧ   = '0' ");
				cmdQuery_h.Append("ORDER BY 記事ＣＤ \n");
				OracleDataReader reader_h = CmdSelect(sUser, conn2, cmdQuery_h);

				bool b輸送指示 = true;
				bool b品名記事 = true;
				string s親記事 = "";
				while (true)
				{
					if (b輸送指示) b輸送指示 = reader_y.Read();
					if (b品名記事) b品名記事 = reader_h.Read();

					string[] sData = new string[4];
					if (b輸送指示)
					{
						sData[0]  = reader_y.GetString(0).TrimEnd();
						sData[1]  = reader_y.GetString(1).TrimEnd();
					}
					else
					{
						sData[0] = "";
						sData[1] = "";
					}
					if (b輸送指示 && !sData[0].Equals(s親記事))
					{
						if (b品名記事)
						{
							sData[2]  = reader_h.GetString(0).TrimEnd();
							sData[3]  = reader_h.GetString(1).TrimEnd();
// DEL 2005.06.01 東都）伊賀 輸送商品仕様変更 START
//							if (b品名記事) b品名記事 = reader_h.Read();
// DEL 2005.06.01 東都）伊賀 輸送商品仕様変更 END
						}
						else
						{
							sData[2] = "";
							sData[3] = "";
						}
						s親記事 = sData[0];
						alRet.Add(sData);
// MOD 2005.06.01 東都）伊賀 輸送商品仕様変更 START
//						sData = new string[4];
//						sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
//						sData[1]  = "　　　" + reader_y.GetString(3).TrimEnd();
						if (!reader_y.GetString(2).TrimEnd().Equals(""))
						{
							sData = new string[4];
							if (b品名記事) b品名記事 = reader_h.Read();
							sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
							sData[1]  = "　　　" + reader_y.GetString(3).TrimEnd();
						}
						else
						{
							continue;
						}
// MOD 2005.06.01 東都）伊賀 輸送商品仕様変更 START
					}
					else
					{
						if (b輸送指示)
						{
							sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
							sData[1]  = "　　　" + reader_y.GetString(3).TrimEnd();
						}
					}

					if (b品名記事)
					{
						sData[2]  = reader_h.GetString(0).TrimEnd();
						sData[3]  = reader_h.GetString(1).TrimEnd();
					}
					else
					{
						sData[2] = "";
						sData[3] = "";
					}
					if (!b輸送指示 && !b品名記事) break;
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader_y);
				disposeReader(reader_h);
				reader_y = null;
				reader_h = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * 未出荷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、登録日、ジャーナルＮＯ
		 *
		 *********************************************************************/
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 START
// ADD 2005.05.11 東都）高木 ORA-03113対策？ START
		private static string GET_UNPUBLISHED_SELECT
			= "SELECT 登録日, \"ジャーナルＮＯ\" \n"
			+ " FROM \"ＳＴ０１出荷ジャーナル\" \n";
		private static string GET_UNPUBLISHED_SELECT_WHERE
			= " AND 送り状発行済ＦＧ = '0' \n"
			+ " AND 状態     = '01' \n"
			+ " AND 削除ＦＧ = '0' \n"
// ADD 2005.05.11 東都）高木 ORA-03113対策？ END
// ADD 2005.07.11 東都）高木 ラベルの印刷順の設定 START
			+ " ORDER BY 出荷日, 登録日, \"ジャーナルＮＯ\" \n";
// ADD 2005.07.11 東都）高木 ラベルの印刷順の設定 END
//		private static string GET_UNPUBLISHED_SELECT
//			= "SELECT /*+ INDEX(ST01 ST01IDX2) INDEX(SM01 SM01PKEY) */ \n"
//			+ " ST01.登録日, ST01.\"ジャーナルＮＯ\" \n"
//			+ " FROM \"ＳＴ０１出荷ジャーナル\" ST01, ＳＭ０１荷送人 SM01 \n";
//		private static string GET_UNPUBLISHED_SELECT_WHERE
//			= " AND ST01.送り状発行済ＦＧ = '0' \n"
//			+ " AND ST01.状態     = '01' \n"
//			+ " AND ST01.削除ＦＧ = '0' \n"
//			+ " AND ST01.会員ＣＤ = SM01.会員ＣＤ \n"
//			+ " AND ST01.部門ＣＤ = SM01.部門ＣＤ \n"
//			+ " AND ST01.荷送人ＣＤ = SM01.荷送人ＣＤ \n"
//			+ " AND SM01.削除ＦＧ = '0' \n"
//			+ " ORDER BY ST01.出荷日, ST01.登録日, ST01.\"ジャーナルＮＯ\" \n";
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 END
		[WebMethod]
		public ArrayList Get_Unpublished(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "未出荷データ検索開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			try
			{
				//未発行データを取得
				//StringBuilder使用
				System.Text.StringBuilder cmdQuery = new System.Text.StringBuilder(256);
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//				cmdQuery.Append("SELECT ");
//				cmdQuery.Append( "登録日 ");
//				cmdQuery.Append(",\"ジャーナルＮＯ\" ");
//				cmdQuery.Append(  "FROM \"ＳＴ０１出荷ジャーナル\" ");
//				cmdQuery.Append( "WHERE 会員ＣＤ = '" + sKey[0] + "' ");
//				cmdQuery.Append(   "AND 部門ＣＤ = '" + sKey[1] + "' ");
//				cmdQuery.Append(   "AND 送り状発行済ＦＧ = '0' ");
//				cmdQuery.Append(   "AND 状態 = '01' ");
//				cmdQuery.Append(   "AND 削除ＦＧ = '0' \n");
				cmdQuery.Append(GET_UNPUBLISHED_SELECT);
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 START
				cmdQuery.Append(" WHERE 会員ＣＤ = '" + sKey[0] + "' \n");
				cmdQuery.Append(  " AND 部門ＣＤ = '" + sKey[1] + "' \n");
//				cmdQuery.Append(" WHERE ST01.会員ＣＤ = '" + sKey[0] + "' \n");
//				cmdQuery.Append(  " AND ST01.部門ＣＤ = '" + sKey[1] + "' \n");
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 END
				cmdQuery.Append(GET_UNPUBLISHED_SELECT_WHERE);
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				cmdQuery = null;

				while (reader.Read())
				{
					string[] sData = new string[3];
					sData[0]  = reader.GetString(0).Trim();
					sData[1]  = reader.GetDecimal(1).ToString().Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "送り状はすべて印刷済です。";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * 登録済み送り状未発行データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、登録日、ジャーナルＮＯ
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ShippedUnpublished(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "送り状未発行検索開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			try
			{
				//登録済み、送り状未発行のデータを取得
				//StringBuilder使用
				System.Text.StringBuilder cmdQuery = new System.Text.StringBuilder(256);
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 START
				cmdQuery.Append("SELECT ");
				cmdQuery.Append( "登録日 ");
				cmdQuery.Append(",\"ジャーナルＮＯ\" ");
				cmdQuery.Append(  "FROM \"ＳＴ０１出荷ジャーナル\" ");
				cmdQuery.Append( "WHERE 会員ＣＤ = '" + sKey[0] + "' ");
				cmdQuery.Append(   "AND 部門ＣＤ = '" + sKey[1] + "' ");
				cmdQuery.Append(   "AND 送り状発行済ＦＧ = '0' ");
				cmdQuery.Append(   "AND 状態 = '01' ");
				cmdQuery.Append(   "AND 削除ＦＧ = '0' \n");
// ADD 2005.07.11 東都）高木 ラベルの印刷順の設定 START
				cmdQuery.Append(" ORDER BY 出荷日, 登録日, \"ジャーナルＮＯ\" \n");
// ADD 2005.07.11 東都）高木 ラベルの印刷順の設定 END
//				//未出荷データ検索と同じＳＱＬ
//				cmdQuery.Append(GET_UNPUBLISHED_SELECT);
//				cmdQuery.Append(" WHERE ST01.会員ＣＤ = '" + sKey[0] + "' \n");
//				cmdQuery.Append(" AND ST01.部門ＣＤ = '" + sKey[1] + "' \n");
//				cmdQuery.Append(GET_UNPUBLISHED_SELECT_WHERE);
//保留 MOD 2007.05.29 東都）高木 未出荷データ数の不一致障害 END
				/*
								//string使用
								string cmdQuery = "SELECT "
												+  "登録日 "
												+  ",ジャーナルＮＯ "
												+   "FROM \"ＳＴ０１出荷ジャーナル\" "
												+  "WHERE 会員ＣＤ = '" + sKey[0] + "' "
												+    "AND 部門ＣＤ = '" + sKey[1] + "' "
												+    "AND 送り状発行済ＦＧ = '0' "
												+    "AND 状態 = '01' "
												+    "AND 削除ＦＧ = '0' ";
				*/
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				cmdQuery = null;

				while (reader.Read())
				{
					string[] sData = new string[3];
					sData[0]  = reader.GetString(0).Trim();
					sData[1]  = reader.GetDecimal(1).ToString().Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "送り状はすべて印刷済です。";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}

// ADD 2005.06.06 東都）小童谷 出荷実績印刷データ取得 START
		/*********************************************************************
		 * 出荷実績印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日
		 * 戻値：ステータス、登録日、荷受人ＣＤ...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_1
			= "SELECT J.登録日, J.出荷日, 送り状番号, J.荷受人ＣＤ, J.郵便番号, \n"
			+       " J.電話番号１, J.電話番号２, J.電話番号３, \n"
			+       " J.住所１, J.住所２, J.住所３, J.名前１, J.名前２, J.着店ＣＤ, J.着店名, \n"
			+       " J.荷送人ＣＤ, NVL(N.郵便番号, ' '), \n"
			+       " NVL(N.電話番号１,' '), NVL(N.電話番号２,' '), NVL(N.電話番号３,' '), \n"
			+       " NVL(N.住所１,' '), NVL(N.住所２,' '), NVL(N.名前１,' '), NVL(N.名前２,' '), \n"
			+       " J.荷送人部署名, TO_CHAR(J.個数), TO_CHAR(J.重量), \n"
			+       " J.指定日, J.輸送指示１, J.輸送指示２, J.品名記事１, J.品名記事２, J.品名記事３, \n"
			+       " J.元着区分, TO_CHAR(J.保険金額), J.お客様出荷番号, \n"
			+       " TO_CHAR(J.才数), J.指定日区分 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			+       ", J.運賃才数, J.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" J,ＳＭ０１荷送人 N \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
			;

		private static string GET_SYUKKA_SELECT_1_SORT
			= " ORDER BY 登録日,\"ジャーナルＮＯ\" ";

		private static string GET_SYUKKA_SELECT_1_SORT2
			= " ORDER BY 出荷日,登録日,\"ジャーナルＮＯ\" ";

		[WebMethod]
		public ArrayList Get_PublishedPrintData(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷実績印刷データ開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
//			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数 = 0;
			decimal d重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.部門ＣＤ = '" + sBCode + "' \n");

				if(iSyuka == 0)
					sbQuery.Append(" AND J.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				sbQuery.Append(" AND J.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND J.荷送人ＣＤ     = N.荷送人ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.会員ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.部門ＣＤ(+) \n");
				sbQuery.Append(" AND '0' = N.削除ＦＧ(+) ");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
				sbQuery.Append(" AND J.会員ＣＤ     = CM01.会員ＣＤ(+) \n");
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

				OracleDataReader reader;
				if(iSyuka == 0)
				{
					sbQuery2.Append(GET_SYUKKA_SELECT_1);
					sbQuery2.Append(sbQuery);
					sbQuery2.Append(GET_SYUKKA_SELECT_1_SORT2);
					reader = CmdSelect(sUser, conn2, sbQuery2);
				}
				else
				{
					sbQuery2.Append(GET_SYUKKA_SELECT_1);
					sbQuery2.Append(sbQuery);
					sbQuery2.Append(GET_SYUKKA_SELECT_1_SORT);
					reader = CmdSelect(sUser, conn2, sbQuery2);
				}

				while (reader.Read())
				{
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					string[] sData = new string[38];
					string[] sData = new string[40];
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					for(int iCnt = 0; iCnt < 38; iCnt++)
					for(int iCnt = 0; iCnt < sData.Length; iCnt++)
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
					{
// ADD 2005.09.02 東都）小童谷 Trimはずし START
//						sData[iCnt] = reader.GetString(iCnt).Trim();
						sData[iCnt] = reader.GetString(iCnt);
// ADD 2005.09.02 東都）小童谷 Trimはずし END
					}
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
					s運賃才数 = reader.GetString(38).TrimEnd();
					s運賃重量 = reader.GetString(39).TrimEnd();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(40).TrimEnd();
					if(s重量入力制御 == "1"
					&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//					&& (sData[26].TrimEnd() != "0" || sData[36].TrimEnd() != "0")
					){
						;
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						d才数 = 0;
						d重量 = 0;
						if(s運賃才数.Length > 0){
							try{
								d才数 = Decimal.Parse(s運賃才数);
							}catch(Exception){}
						}
						if(s運賃重量.Length > 0){
							try{
								d重量 = Decimal.Parse(s運賃重量);
							}catch(Exception){}
						}
						sData[26] = d重量.ToString();	// 重量
						sData[36] = d才数.ToString();	// 才数
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2005.06.06 東都）小童谷 出荷実績印刷データ取得 END

// ADD 2006.07.05 東都）山本 アドレス帳からの届け先印刷データ取得呼び出し対応 START
		/*********************************************************************
		 * 届け先印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、荷受人ＣＤ、カナ略称、電話番号...
		 *
		 *********************************************************************/
		[WebMethod]
// ADD 2007.02.14 FJCS）桑田 検索条件に名前の追加 START
		public ArrayList Get_ConsigneePrintData2(string[] sUser, string[] sKey, string sKana, string sTCode, string sTelNo, string sTelNo2, string sTelNo3, string sName,int iSortLabel1,int iSortPat1,int iSortLabel2,int iSortPat2)
// MOD 2010.02.03 東都）高木 検索条件に更新日を追加 START
		{
			return Get_ConsigneePrintData3(
				sUser, sKey, sKana, sTCode, sTelNo, sTelNo2, sTelNo3
				, sName, iSortLabel1, iSortPat1, iSortLabel2, iSortPat2
				, ""
			);
		}

		[WebMethod]
		public ArrayList Get_ConsigneePrintData3(
			string[] sUser, string[] sKey, string sKana, string sTCode
			, string sTelNo, string sTelNo2, string sTelNo3
			, string sName,int iSortLabel1,int iSortPat1,int iSortLabel2,int iSortPat2
			, string sUpdateDay)
// MOD 2010.02.03 東都）高木 検索条件に更新日を追加 END
// ADD 2007.02.14 FJCS）桑田 検索条件に名前の追加 END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "届け先印刷データ取得開始");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT");
				sbQuery.Append(" 荷受人ＣＤ ");
				sbQuery.Append(",カナ略称 ");
				sbQuery.Append(",電話番号１ ");
				sbQuery.Append(",電話番号２ ");
				sbQuery.Append(",電話番号３ ");
				sbQuery.Append(",郵便番号 ");
				sbQuery.Append(",住所１ ");
				sbQuery.Append(",住所２ ");
				sbQuery.Append(",住所３ ");
				sbQuery.Append(",名前１ ");
				sbQuery.Append(",名前２ ");
				sbQuery.Append(",特殊計 \n");
				sbQuery.Append(" FROM \"ＳＭ０２荷受人\" \n");
				sbQuery.Append(" WHERE 会員ＣＤ = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND 部門ＣＤ = '" + sKey[1] + "' \n");
				if(sKana.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND カナ略称 LIKE '%"+ sKana + "%' \n");
				}
				if(sTCode.Length > 0 && sKana.Length == 0)
				{
					sbQuery.Append(" AND 荷受人ＣＤ LIKE '"+ sTCode + "%' \n");
				}
				if(sTCode.Length > 0 && sKana.Length > 0)
				{
					sbQuery.Append(" AND カナ略称 LIKE '%"+ sKana + "%' \n");
					sbQuery.Append(" AND 荷受人ＣＤ LIKE '"+ sTCode + "%' \n");
				}
				if(sTelNo.Length > 0)
				{
					sbQuery.Append(" AND 電話番号１ LIKE '"+ sTelNo + "%' \n");
				}
				if(sTelNo2.Length > 0)
				{
					sbQuery.Append(" AND 電話番号２ LIKE '"+ sTelNo2 + "%' \n");
				}
				if(sTelNo3.Length > 0)
				{
					sbQuery.Append(" AND 電話番号３ LIKE '"+ sTelNo3 + "%' \n");
				}
				// ADD 2007.01.30 FJCS）桑田 検索条件に名前を追加 START
				if(sName.Length > 0)
				{
					sbQuery.Append(" AND 名前１ LIKE '%"+ sName + "%' \n");
				}
				// ADD 2007.01.30 FJCS）桑田 検索条件に名前を追加 END
// MOD 2010.02.03 東都）高木 検索条件に更新日を追加 START
				if(sUpdateDay.Length > 0){
					string s更新日時Ｓ = sUpdateDay + "000000";
					string s更新日時Ｅ = sUpdateDay + "999999";
					sbQuery.Append(" AND 更新日時 BETWEEN "+s更新日時Ｓ+" AND "+s更新日時Ｅ+" \n");
				}
// MOD 2010.02.03 東都）高木 検索条件に更新日を追加 END
				sbQuery.Append(" AND 削除ＦＧ = '0' \n");

// MOD 2009.01.29 東都）高木 一覧のソート順に[荷受人ＣＤ]を追加 START
//				if((iSortLabel1 != 0)||(iSortLabel2 != 0))
//					sbQuery.Append(" ORDER BY \n");
				sbQuery.Append(" ORDER BY \n");
// MOD 2009.01.29 東都）高木 一覧のソート順に[荷受人ＣＤ]を追加 END
				if(iSortLabel1 != 0)
				{
					switch(iSortLabel1)
					{
// UPD 2007.01.30 FJCS）桑田 Index項目変更 START
//						case 1:
//							sbQuery.Append(" 名前１ ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 2:
//							sbQuery.Append(" 荷受人ＣＤ ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 3:
//							sbQuery.Append(" 電話番号１ ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", 電話番号２ ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", 電話番号３ ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 4:
//							sbQuery.Append(" カナ略称 ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 5:
//							sbQuery.Append(" 登録日時 ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 6:
//							sbQuery.Append(" 更新日時");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
						case 1:
							sbQuery.Append(" カナ略称 ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 2:
							sbQuery.Append(" 荷受人ＣＤ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 3:
							sbQuery.Append(" 電話番号１ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", 電話番号２ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", 電話番号３ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 4:
							sbQuery.Append(" 名前１ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 東都）高木 一覧のソート順に[名前２]を追加 START
							sbQuery.Append(", 名前２ ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 東都）高木 一覧のソート順に[名前２]を追加 END
							break;
						case 5:
							sbQuery.Append(" 登録日時 ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 6:
							sbQuery.Append(" 更新日時");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
// UPD 2007.01.30 FJCS）桑田 Index項目変更 END
					}
					if(iSortLabel2 != 0)
						sbQuery.Append(" , ");
				}
				if(iSortLabel2 != 0)
				{
					switch(iSortLabel2)
					{
// UPD 2007.01.30 FJCS）桑田 Index項目変更 START
//						case 1:
//							sbQuery.Append(" 名前１ ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 2:
//							sbQuery.Append(" 荷受人ＣＤ ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 3:
//							sbQuery.Append(" 電話番号１ ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", 電話番号２ ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", 電話番号３ ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 4:
//							sbQuery.Append(" カナ略称 ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 5:
//							sbQuery.Append(" 登録日時 ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 6:
//							sbQuery.Append(" 更新日時");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
						case 1:
							sbQuery.Append(" カナ略称 ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 2:
							sbQuery.Append(" 荷受人ＣＤ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 3:
							sbQuery.Append(" 電話番号１ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", 電話番号２ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", 電話番号３ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 4:
							sbQuery.Append(" 名前１ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 東都）高木 一覧のソート順に[名前２]を追加 START
							sbQuery.Append(", 名前２ ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 東都）高木 一覧のソート順に[名前２]を追加 END
							break;
						case 5:
							sbQuery.Append(" 登録日時 ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 6:
							sbQuery.Append(" 更新日時");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
// UPD 2007.01.30 FJCS）桑田 Index項目変更 END
					}
				}
// ADD 2009.01.29 東都）高木 一覧のソート順に[荷受人ＣＤ]を追加 START
				if((iSortLabel1 != 0) || (iSortLabel2 != 0))
					sbQuery.Append(" , ");
				sbQuery.Append(" 荷受人ＣＤ \n");
// ADD 2009.01.29 東都）高木 一覧のソート順に[荷受人ＣＤ]を追加 END

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
					string[] sData = new string[13];
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // カナ略称
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); //住所１
					sData[8]  = reader.GetString(7).TrimEnd(); //住所２
					sData[9]  = reader.GetString(8).TrimEnd(); //住所３
					sData[10] = reader.GetString(9).TrimEnd(); //名前１
					sData[11] = reader.GetString(10).TrimEnd();//名前２
// MOD 2011.01.18 東都）高木 住所名前の前SPACEをつめない END
					sData[12] = reader.GetString(11).Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2006.07.05 東都）山本 アドレス帳からの届け先印刷データ取得呼び出し対応 START

// ADD 2006.08.03 東都）山本 出力結果に運賃項目を追加 START
		/*********************************************************************
		 * 出荷実績印刷データ取得
		 * 引数：会員ＣＤ、部門ＣＤ、出荷日 or 登録日、
		 *		 開始日、終了日
		 * 戻値：ステータス、登録日、荷受人ＣＤ...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_2
			= "SELECT J.登録日, J.出荷日, 送り状番号, J.荷受人ＣＤ, J.郵便番号, \n"
			+       " J.電話番号１, J.電話番号２, J.電話番号３, \n"
			+       " J.住所１, J.住所２, J.住所３, J.名前１, J.名前２, J.着店ＣＤ, J.着店名, \n"
			+       " J.荷送人ＣＤ, NVL(N.郵便番号, ' '), \n"
			+       " NVL(N.電話番号１,' '), NVL(N.電話番号２,' '), NVL(N.電話番号３,' '), \n"
			+       " NVL(N.住所１,' '), NVL(N.住所２,' '), NVL(N.名前１,' '), NVL(N.名前２,' '), \n"
			+       " J.荷送人部署名, TO_CHAR(J.個数), TO_CHAR(J.重量), \n"
			+       " J.指定日, J.輸送指示１, J.輸送指示２, J.品名記事１, J.品名記事２, J.品名記事３, \n"
			+       " J.元着区分, TO_CHAR(J.保険金額), J.お客様出荷番号, \n"
//			+       " TO_CHAR(J.才数), J.指定日区分 \n"
			+       " TO_CHAR(J.才数), J.指定日区分 ,\n"
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 START
//			+       " TO_CHAR(J.運賃) \n"
			+       " TO_CHAR(J.運賃 + J.中継) \n"
// MOD 2007.10.22 東都）高木 運賃に中継料を加算表示 END
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+       ", NVL(CM01.記事連携ＦＧ,'0') \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
			+       ", J.得意先ＣＤ, J.部課ＣＤ, J.部課名 \n"
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			+       ", J.運賃才数, J.運賃重量 \n"
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			+       ", NVL(CM01.保留印刷ＦＧ,'0') \n"
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
			+       ", J.品名記事４, J.品名記事５, J.品名記事６ \n"
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 START
			+       ", J.処理０３ \n"
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 END
			+ " FROM \"ＳＴ０１出荷ジャーナル\" J,ＳＭ０１荷送人 N \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
			+  ", ＣＭ０１会員 CM01 \n"
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
			;

		private static string GET_SYUKKA_SELECT_2_SORT
			= " ORDER BY 登録日,\"ジャーナルＮＯ\" ";

		private static string GET_SYUKKA_SELECT_2_SORT2
			= " ORDER BY 出荷日,登録日,\"ジャーナルＮＯ\" ";

// ADD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 START
		private static string GET_SYUKKA_SELECT_2_SORT3
			= " ORDER BY 登録日,荷送人ＣＤ,\"ジャーナルＮＯ\" ";

		private static string GET_SYUKKA_SELECT_2_SORT4
			= " ORDER BY 出荷日,荷送人ＣＤ,登録日,\"ジャーナルＮＯ\" ";

// ADD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 END

		[WebMethod]
		public ArrayList Get_PublishedPrintData2(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday, string sIraiCd)
// ADD 2008.07.09 東都）高木 未発行分を除外する START
		{
			return Get_PublishedPrintData3(sUser, sKCode, sBCode, iSyuka, sSday, sEday, sIraiCd, "00");
		}

		[WebMethod]
		public ArrayList Get_PublishedPrintData3(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday, string sIraiCd, string sJyoutai)
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
		{
			string[] sKey = new string[]{sKCode, sBCode, "", sIraiCd, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_PublishedPrintData4(sUser, sKey);
		}
		
		[WebMethod]
		public ArrayList Get_PublishedPrintData4(string[] sUser, string[] sKey)
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
// ADD 2008.07.09 東都）高木 未発行分を除外する END
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "出荷実績印刷データ４開始");
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string s荷受人ＣＤ = sKey[2];
			string sIraiCd  = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s送り状番号開始 = ""; if(sKey.Length >  8) s送り状番号開始 = sKey[ 8];
			string s送り状番号終了 = ""; if(sKey.Length >  9) s送り状番号終了 = sKey[ 9];
			string sお客様番号開始 = ""; if(sKey.Length > 10) sお客様番号開始 = sKey[10];
			string sお客様番号終了 = ""; if(sKey.Length > 11) sお客様番号終了 = sKey[11];
			string s請求先ＣＤ     = ""; if(sKey.Length > 12) s請求先ＣＤ     = sKey[12];
			string s請求先部課ＣＤ = ""; if(sKey.Length > 13) s請求先部課ＣＤ = sKey[13];
			int    i出力形式       = 0 ; if(sKey.Length > 14) i出力形式       = int.Parse(sKey[14]);

			if(s送り状番号開始.Length == 0) s送り状番号開始 = s送り状番号終了;
			if(s送り状番号終了.Length == 0) s送り状番号終了 = s送り状番号開始;
			if(sお客様番号開始.Length == 0) sお客様番号開始 = sお客様番号終了;
			if(sお客様番号終了.Length == 0) sお客様番号終了 = sお客様番号開始;
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();

			string[] sRet = new string[1];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}

// MOD 2011.04.13 東都）高木 重量入力不可対応 START
			string  s運賃才数 = "";
			string  s運賃重量 = "";
			decimal d才数 = 0;
			decimal d重量 = 0;
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
			string  s重量入力制御 = "0";
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.会員ＣＤ = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.部門ＣＤ = '" + sBCode + "' \n");
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
				if(s送り状番号開始.Length > 0){
					if(s送り状番号開始 == s送り状番号終了){
						sbQuery.Append(" AND J.送り状番号 = '0000"+ s送り状番号開始 + "' \n");
					}else{
						sbQuery.Append(" AND J.送り状番号 BETWEEN '0000"+ s送り状番号開始
													 + "' AND '0000"+ s送り状番号終了 + "' \n");
					}
				}
				if(sお客様番号開始.Length > 0){
					if(sお客様番号開始 == sお客様番号終了){
						sbQuery.Append(" AND J.お客様出荷番号 = '"+ sお客様番号開始 + "' \n");
					}else{
						sbQuery.Append(" AND J.お客様出荷番号 BETWEEN '"+ sお客様番号開始
													 + "' AND '"+ sお客様番号終了 + "' \n");
					}
				}
				if(s請求先ＣＤ.Length > 0){
					sbQuery.Append(" AND J.得意先ＣＤ = '"+ s請求先ＣＤ + "' \n");
				}
				if(s請求先部課ＣＤ.Length > 0){
					sbQuery.Append(" AND J.部課ＣＤ = '"+ s請求先部課ＣＤ + "' \n");
				}
				if(s荷受人ＣＤ.Length > 0){
					sbQuery.Append(" AND J.荷受人ＣＤ = '"+ s荷受人ＣＤ + "' \n");
				}
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END

// MOD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 START
//				if(iSyuka == 0)
				if(iSyuka == 0 || iSyuka == 2)
// MOD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 END
					sbQuery.Append(" AND J.出荷日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.登録日  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sIraiCd.Trim().Length != 0)
				{
					sbQuery.Append(" AND '" + sIraiCd + "' = J.荷送人ＣＤ(+) \n");
				}
				
				sbQuery.Append(" AND J.削除ＦＧ = '0' \n");
				sbQuery.Append(" AND J.荷送人ＣＤ     = N.荷送人ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.会員ＣＤ(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.部門ＣＤ(+) \n");
				sbQuery.Append(" AND '0' = N.削除ＦＧ(+) ");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
				sbQuery.Append(" AND J.会員ＣＤ = CM01.会員ＣＤ(+) \n");
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
// ADD 2008.07.09 東都）高木 未発行分を除外する START
				if(sJyoutai != "00"){
					if(sJyoutai == "aa"){
						sbQuery.Append(" AND J.状態 <> '01' \n");
					}else{
						sbQuery.Append(" AND J.状態 = '"+ sJyoutai + "' \n");
					}
				}
// ADD 2008.07.09 東都）高木 未発行分を除外する END

				OracleDataReader reader;
// MOD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 START
//				if(iSyuka == 0)
//				{
//					sbQuery2.Append(GET_SYUKKA_SELECT_2);
//					sbQuery2.Append(sbQuery);
//					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
//					reader = CmdSelect(sUser, conn2, sbQuery2);
//				}
//				else
//				{
//					sbQuery2.Append(GET_SYUKKA_SELECT_2);
//					sbQuery2.Append(sbQuery);
//					sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
//					reader = CmdSelect(sUser, conn2, sbQuery2);
//				}
				sbQuery2.Append(GET_SYUKKA_SELECT_2);
				sbQuery2.Append(sbQuery);
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
				switch(i出力形式){
				case 1:		//ご依頼主別
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT4);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT3);
					}
					break;
				case 2:		//請求先別
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY 出荷日,得意先ＣＤ,部課ＣＤ,登録日,\"ジャーナルＮＯ\" ");
					}else{
						sbQuery2.Append(" ORDER BY 登録日,得意先ＣＤ,部課ＣＤ,\"ジャーナルＮＯ\" ");
					}
					break;
				case 3:		//お届け先別
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY 出荷日,荷受人ＣＤ,登録日,\"ジャーナルＮＯ\" ");
					}else{
						sbQuery2.Append(" ORDER BY 登録日,荷受人ＣＤ,\"ジャーナルＮＯ\" ");
					}
					break;
				default:	//指定なし
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
					}else if(iSyuka == 2){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT4);
					}else if(iSyuka == 3){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT3);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
					}
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
					break;
				}
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
				reader = CmdSelect(sUser, conn2, sbQuery2);
// MOD 2009.02.02 東都）高木 実績一覧のソート順に[荷送人ＣＤ]を追加 END

				while (reader.Read())
				{
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
//					string[] sData = new string[39];
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
//					string[] sData = new string[42];
// MOD 2011.07.14 東都）高木 記事行の追加 START
//					string[] sData = new string[44];
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 START
//					string[] sData = new string[47];
					string[] sData = new string[48];
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 END
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
					for(int iCnt = 0; iCnt < 39; iCnt++)
					{
						sData[iCnt] = reader.GetString(iCnt);
					}
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 START
					if(reader.GetString(39).Equals("1")){
						sData[38] = "0";
					}
// MOD 2009.05.28 東都）高木 出荷実績一覧運賃非表示対応 END
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 START
					sData[39] = reader.GetString(40);	//得意先ＣＤ
					sData[40] = reader.GetString(41);	//部課ＣＤ
					sData[41] = reader.GetString(42);	//部課名
// MOD 2009.11.06 東都）高木 検索条件に請求先、お届け先、お客様番号を追加 END
// MOD 2011.04.13 東都）高木 重量入力不可対応 START
					sData[42] = reader.GetString(43);
					sData[43] = reader.GetString(44);
					s運賃才数 = reader.GetString(43).TrimEnd();
					s運賃重量 = reader.GetString(44).TrimEnd();
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					s重量入力制御 = reader.GetString(45).TrimEnd();
					if(s重量入力制御 == "1"
					&& s運賃才数.Length == 0 && s運賃重量.Length == 0
//					&& (sData[26].TrimEnd() != "0" || sData[36].TrimEnd() != "0")
					){
						sData[42] = sData[36]; // 才数
						sData[43] = sData[26]; // 重量
					}else{
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
//保留						if(s重量入力制御 == "1"
//保留						&& sData[42] == "     " && sData[43] == "     "
//保留						){
//保留							sData[42] = sData[36]; // 才数
//保留							sData[43] = sData[26]; // 重量
//保留						}
//保留 MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
						d才数 = 0;
						d重量 = 0;
						if(s運賃才数.Length > 0){
							try{
								d才数 = Decimal.Parse(s運賃才数);
							}catch(Exception){}
						}
						if(s運賃重量.Length > 0){
							try{
								d重量 = Decimal.Parse(s運賃重量);
							}catch(Exception){}
						}
						sData[26] = d重量.ToString();	// 重量
						sData[36] = d才数.ToString();	// 才数
// MOD 2011.04.13 東都）高木 重量入力不可対応 END
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 START
					}
// MOD 2011.05.06 東都）高木 お客様ごとに重量入力制御 END
// MOD 2011.07.14 東都）高木 記事行の追加 START
					sData[44] = reader.GetString(46).TrimEnd(); // 品名記事４
					sData[45] = reader.GetString(47).TrimEnd(); // 品名記事５
					sData[46] = reader.GetString(48).TrimEnd(); // 品名記事６
// MOD 2011.07.14 東都）高木 記事行の追加 END
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 START
					sData[47] = reader.GetString(49).Trim();	// 配完日付・時刻
// MOD 2013.10.07 BEVAS）高杉 出荷実績一覧表に配完日付・時刻を追加 END
					alRet.Add(sData);
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				if (alRet.Count == 0)
				{
					sRet[0] = "該当データがありません";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "正常終了";
					alRet.Insert(0, sRet);
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				alRet.Insert(0, sRet);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2006.08.03 東都）山本 出力結果に運賃項目を追加 END


// ADD 2014.09.10 BEVAS)前田 支店止め機能追加対応 START
		/*********************************************************************
		 * 支店止め対応店所一覧取得
		 * 引数：店所ＣＤ／店所名, 福通/王子フラグ 
		 * ("1" = 福山通運からの支店止め対応 / "2" = 王子運輸からの支店止め対応)
		 * 戻値：ステータス、店所ＣＤ、店所名、住所
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_DeliShop(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "支店止め対応店所一覧取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT '|'"
					+     " || TRIM(店所ＣＤ) || '|' "
					+     " || TRIM(店所名) || '|' "
					+     " || TRIM(店所正式名) || '|' "
					+     " || TRIM(郵便番号) || '|' \n"
					+     "\n"
					+  " FROM ＣＭ１０店所 \n";
				
				if (sKey[2].Equals("2")) 
				{
					// 王子運輸からの支店止めに対応する店所を検索
					cmdQuery += " WHERE 支店止めＦＧ２ = '1' \n";
				} 
				else 
				{
					// 福山通運からの支店止めに対応する店所を検索
					cmdQuery += " WHERE 支店止めＦＧ１ = '1' \n";
				}

				if (sKey[0].Length == 4)
				{
					cmdQuery += " AND 店所ＣＤ = '" + sKey[0] + "' \n";
				}
				else if (sKey[0].Length > 0) 
				{
					cmdQuery += " AND 店所ＣＤ LIKE '" + sKey[0] + "%' \n";				
				} 
				else 
				{
					// なにもしない
				}
				if (sKey[1].Length > 0)
				{
					cmdQuery += " AND 店所名 LIKE '" + sKey[1] + "%' \n";
				} 
				else 
				{
					// なにもしない
				}
				cmdQuery += " AND 削除ＦＧ = '0' \n"
					     +  " AND NVL(LENGTH(TRIM(郵便番号)),0) = 7 \n"
						 +  " ORDER BY 店所ＣＤ \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * 店所マスタから店所名及び郵便番号を取得
		 * 引数：店所ＣＤ
		 * 戻値：ステータス、店所ＣＤ，店所名,郵便番号
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ShopZip(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "店所郵便番号取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT "
					+     "   TRIM(店所ＣＤ) "
					+     ",  TRIM(店所名) "
					+     ",  郵便番号 "
					+     "\n"
					+  " FROM ＣＭ１０店所 \n"
					+  " WHERE 店所ＣＤ = '" + sKey[0] + "' \n"
					+  " AND 削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
					sList.Add(reader.GetString(1));
					sList.Add(reader.GetString(2));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * 店所正式名を取得
		 * 引数：店所ＣＤ
		 * 戻値：ステータス、店所正式名
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ShopType(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "支店種別取得開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];

			if (sKey[0].Length != 3) 
			{
				sRet[0] = "店所ＣＤの形式が不正です。";
				return sRet;
			}

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				sRet[1] = "";
				return sRet;
			}

			string cmdQuery = "";
			try
			{				
				cmdQuery
					= "SELECT \n"
					+     " TRIM(店所正式名) \n"
					+  " FROM ＣＭ１０店所 \n"
					+  " WHERE 削除ＦＧ = '0' \n"
					+  " AND LENGTH(TRIM(郵便番号)) = 7 \n"
					+  " AND 店所ＣＤ = '" + sKey[0] + "' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				reader.Read();

				// 店所正式名を返す
				sRet[1] = reader.GetString(0);

				disposeReader(reader);
				reader = null;
				sRet[0] = "正常終了";
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = "";
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				sRet[1] = "";
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}

		/*********************************************************************
		 * 都道府県毎の支店止め対応店所一覧取得
		 * 引数：都道府県名, 福通/王子フラグ 
		 * ("1" = 福山通運からの支店止め対応 / "2" = 王子運輸からの支店止め対応)
		 * 戻値：ステータス、店所ＣＤ、店所名
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_PrefDeliShop(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "支店止め対応店所一覧取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT '|' "
					+     " || TRIM(店所ＣＤ) || '|' "
					+     " || TRIM(店所名) || '|' "
					+     " || TRIM(店所正式名) || '|' "
					+     " || TRIM(郵便番号) || '|' \n"
					+  " FROM ＣＭ１０店所 \n"
					+ " WHERE 住所 LIKE '" + sKey[0] + "%' \n";
				if (sKey[1].Equals("2")) 
				{
					// 王子運輸からの支店止めに対応する店所を検索
					cmdQuery += " AND 支店止めＦＧ２ = '1' \n";
				} 
				else 
				{
					// 福山通運からの支店止めに対応する店所を検索
					cmdQuery += " AND 支店止めＦＧ１ = '1' \n";
				}
				cmdQuery += " AND 削除ＦＧ = '0' \n"
					     +  " AND LENGTH(TRIM(郵便番号)) = 7 \n"
						 +  " ORDER BY 店所ＣＤ \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					int iCnt = 1;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// ADD 2014.09.10 BEVAS)前田 支店止め機能追加対応 END

	}
}
