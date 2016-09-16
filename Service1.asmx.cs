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
	// �C������
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j��
	//	disposeReader(reader);
	//	reader = null;
	// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q
	// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\��
	//--------------------------------------------------------------------------
	// DEL 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX�ɔ����A���X��\���@�\���폜
	//  Get_InvoicePrintData
	// ADD 2008.07.09 ���s�j���� �����s�������O���� 
	//--------------------------------------------------------------------------
	// ADD 2009.01.29 ���s�j���� ���͂���̈ꗗ�̃\�[�g����[�׎�l�b�c]��ǉ� 
	// ADD 2009.01.29 ���s�j���� ���͂���̈ꗗ�̃\�[�g����[���O�Q]��ǉ� 
	// ADD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� 
	// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� 
	// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� 
	//--------------------------------------------------------------------------
	// MOD 2010.02.03 ���s�j���� ���������ɍX�V����ǉ� 
	// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� 
	// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� 
	//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� 
	// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� 
	// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V 
	//--------------------------------------------------------------------------
	// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� 
	// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� 
	// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� 
	// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ 
	// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� 
	// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� 
	// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� 
	// MOD 2011.10.06 ���s�j���� �o�׃f�[�^�̈�����O�̒ǉ� 
	// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ 
	//--------------------------------------------------------------------------
	// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ�
	//--------------------------------------------------------------------------
	// MOD 2014.09.25 BEVAS�j�O�c �x�X�~�ߑΉ�
	//--------------------------------------------------------------------------
	// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή�
	//--------------------------------------------------------------------------
	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2print")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * �o�׈���f�[�^�擾
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n
		 * �ߒl�F�X�e�[�^�X�A�׎�l�b�c�A�d�b�ԍ��A�Z��...
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_InvoicePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׈���f�[�^�擾�J�n");

// MOD 2005.06.08 ���s�j�ɉ�@�w����敪�ǉ� START
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� START
//			string[] sRet = new string[34];
//			string[] sRet = new string[36];
			OracleConnection conn2 = null;
// MOD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� START
//			string[] sRet = new string[37];
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V START
//			string[] sRet = new string[39];
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� START
//			string[] sRet = new string[40];
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//			string[] sRet = new string[41];
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//			string[] sRet = new string[42];
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ START
//			string[] sRet = new string[45];
			string[] sRet = new string[46];
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� END
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V END
// MOD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� END
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� END
// MOD 2005.06.08 ���s�j�ɉ�@�w����敪�ǉ� END
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
			string s���p�ҕ���X���b�c = (sKey.Length >  4) ? sKey[ 4] : "";
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

// ADD 2005.05.18 ���s�j�����J �ː��ǉ� START
			decimal d�ː� = 0;
// ADD 2005.05.18 ���s�j�����J �ː��ǉ� END
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� START
			string s�X�֔ԍ� = "";
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� END
			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT ");
				sbQuery.Append(" ST01.�׎�l�b�c ");
				sbQuery.Append(",ST01.�d�b�ԍ��P ");
				sbQuery.Append(",ST01.�d�b�ԍ��Q ");
				sbQuery.Append(",ST01.�d�b�ԍ��R ");
				sbQuery.Append(",ST01.�Z���P ");
				sbQuery.Append(",ST01.�Z���Q ");
				sbQuery.Append(",ST01.�Z���R ");
				sbQuery.Append(",ST01.���O�P ");
				sbQuery.Append(",ST01.���O�Q ");
				sbQuery.Append(",ST01.�o�ד� ");
				sbQuery.Append(",ST01.�����ԍ� ");
				sbQuery.Append(",ST01.�X�֔ԍ� ");
				sbQuery.Append(",ST01.���X�b�c ");
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� START
//				sbQuery.Append(",ST01.���X�b�c ");
				sbQuery.Append(",NVL(CM14.�X���b�c, ST01.���X�b�c)");
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� END
				sbQuery.Append(",SM01.�d�b�ԍ��P ");
				sbQuery.Append(",SM01.�d�b�ԍ��Q ");
				sbQuery.Append(",SM01.�d�b�ԍ��R ");
				sbQuery.Append(",SM01.�Z���P ");
				sbQuery.Append(",SM01.�Z���Q ");
				sbQuery.Append(",SM01.�Z���R ");
				sbQuery.Append(",SM01.���O�P ");
				sbQuery.Append(",SM01.���O�Q ");
				sbQuery.Append(",ST01.�� ");
				sbQuery.Append(",ST01.�d�� ");
				sbQuery.Append(",ST01.�ی����z ");
				sbQuery.Append(",ST01.�w��� ");
				sbQuery.Append(",ST01.�A���w���P ");
				sbQuery.Append(",ST01.�A���w���Q ");
				sbQuery.Append(",ST01.�i���L���P ");
				sbQuery.Append(",ST01.�i���L���Q ");
				sbQuery.Append(",ST01.�i���L���R ");
				sbQuery.Append(",ST01.�����敪 ");
				sbQuery.Append(",ST01.����󔭍s�ςe�f ");
// ADD 2005.05.18 ���s�j�����J �ː��ǉ� START
				sbQuery.Append(",ST01.�ː� \n");
// ADD 2005.05.18 ���s�j�����J �ː��ǉ� END
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� START
				sbQuery.Append(",ST01.�ב��l������ ");
				sbQuery.Append(",ST01.���q�l�o�הԍ� ");
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� END
// ADD 2005.06.07 ���s�j�ɉ� �A�����i�R�[�h�ʑΉ��ǉ� START
				sbQuery.Append(",ST01.�A���w���b�c�P ");
				sbQuery.Append(",ST01.�A���w���b�c�Q ");
// ADD 2005.06.07 ���s�j�ɉ� �A�����i�R�[�h�ʑΉ��ǉ� END
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
				sbQuery.Append(",ST01.�w����敪 ");
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� START
				sbQuery.Append(",ST01.�X�֔ԍ� ");
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� END
// ADD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� START
				sbQuery.Append(",ST01.�d���b�c ");
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� START
//				sbQuery.Append(",ST01.���X�� ");
				sbQuery.Append(",NVL(CM10.�X����, ST01.���X��)");
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� END
// ADD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� END
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V START
				sbQuery.Append(",ST01.�o�׍ςe�f ");
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V END
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� START
				sbQuery.Append(",SM01.�X�֔ԍ� ");
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(",NVL(CM01.�ۗ�����e�f,'0') \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
				sbQuery.Append(",ST01.�i���L���S ,ST01.�i���L���T ,ST01.�i���L���U \n");
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ START
				sbQuery.Append(",ST01.���X�� ");
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ END
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
				sbQuery.Append(",ST01.���X�b�c ");
				sbQuery.Append(",ST01.���X�� ");
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
				sbQuery.Append(" FROM \"�r�s�O�P�o�׃W���[�i��\" ST01");
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� START
				sbQuery.Append("\n");
				sbQuery.Append(" LEFT JOIN �b�l�O�Q���� CM02 \n");
				sbQuery.Append(" ON ST01.����b�c = CM02.����b�c \n");
				sbQuery.Append("AND ST01.����b�c = CM02.����b�c \n");
				sbQuery.Append(" LEFT JOIN �b�l�P�S�X�֔ԍ� CM14 \n");
				sbQuery.Append(" ON CM02.�X�֔ԍ� = CM14.�X�֔ԍ� \n");
				sbQuery.Append(" LEFT JOIN �b�l�P�O�X�� CM10 \n");
				sbQuery.Append(" ON CM14.�X���b�c = CM10.�X���b�c \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(" LEFT JOIN �b�l�O�P��� CM01 \n");
				sbQuery.Append(" ON ST01.����b�c = CM01.����b�c \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� END
				sbQuery.Append(", \"�r�l�O�P�ב��l\" SM01 \n");
				sbQuery.Append(" WHERE ST01.����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ST01.����b�c = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND ST01.�o�^�� = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND ST01.�W���[�i���m�n = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND ST01.����b�c = SM01.����b�c \n");
				sbQuery.Append(" AND ST01.����b�c = SM01.����b�c \n");
				sbQuery.Append(" AND ST01.�ב��l�b�c = SM01.�ב��l�b�c \n");
				sbQuery.Append(" AND ST01.�폜�e�f = '0' \n");
				sbQuery.Append(" AND SM01.�폜�e�f = '0' \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				int iCnt = 0;
				if (reader.Read())
				{
					string s�A�����i�b�c�P = reader.GetString(36).Trim();
					string s�A�����i�b�c�Q = reader.GetString(37).Trim();
					sRet[1]  = reader.GetString(0).Trim();
					sRet[2]  = reader.GetString(1).Trim();
					sRet[3]  = reader.GetString(2).Trim();
					sRet[4]  = reader.GetString(3).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[5]  = reader.GetString(4).Trim();
//					sRet[6]  = reader.GetString(5).Trim();
//					sRet[7]  = reader.GetString(6).Trim();
//					sRet[8]  = reader.GetString(7).Trim();
//					sRet[9]  = reader.GetString(8).Trim();
					sRet[5]  = reader.GetString(4).TrimEnd(); // �׎�l�Z���P
					sRet[6]  = reader.GetString(5).TrimEnd(); // �׎�l�Z���Q
					sRet[7]  = reader.GetString(6).TrimEnd(); // �׎�l�Z���R
					sRet[8]  = reader.GetString(7).TrimEnd(); // �׎�l���O�P
					sRet[9]  = reader.GetString(8).TrimEnd(); // �׎�l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sRet[10] = reader.GetString(9).Trim();
					sRet[11] = reader.GetString(10).Trim();
					sRet[12] = reader.GetString(11).Trim();
					sRet[13] = reader.GetString(12).Trim().PadLeft(4, '0');
					sRet[14] = reader.GetString(13).Trim().PadLeft(4, '0');
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
					//�Г��`�̏ꍇ�́A�o�׃e�[�u���̕��𐳂Ƃ���
					if(s���p�ҕ���X���b�c == "044")
					{
						sRet[14] = reader.GetString(49).Trim().PadLeft(4, '0');
					}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
					sRet[15] = reader.GetString(14).Trim();
					sRet[16] = reader.GetString(15).Trim();
					sRet[17] = reader.GetString(16).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[18] = reader.GetString(17).Trim();
//					sRet[19] = reader.GetString(18).Trim();
//					sRet[20] = reader.GetString(19).Trim();
//					sRet[21] = reader.GetString(20).Trim();
//					sRet[22] = reader.GetString(21).Trim();
					sRet[18] = reader.GetString(17).TrimEnd(); // �ב��l�Z���P
					sRet[19] = reader.GetString(18).TrimEnd(); // �ב��l�Z���Q
					sRet[20] = reader.GetString(19).TrimEnd(); // �ב��l�Z���R
					sRet[21] = reader.GetString(20).TrimEnd(); // �ב��l���O�P
					sRet[22] = reader.GetString(21).TrimEnd(); // �ב��l���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sRet[23] = reader.GetDecimal(22).ToString().Trim();
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//// MOD 2005.05.18 ���s�j�����J �ː��ǉ� START
////					sRet[24] = reader.GetDecimal(23).ToString().Trim();
//					d�ː�    = reader.GetDecimal(33);
//					d�ː�    = d�ː� * 8;
//					if(d�ː� == 0)
//						sRet[24] = reader.GetDecimal(23).ToString().Trim();
//					else
//						sRet[24] = d�ː�.ToString().Trim();
//// MOD 2005.05.18 ���s�j�����J �ː��ǉ� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					if(reader.GetString(44) == "1"){
						d�ː� = reader.GetDecimal(33) * 8;
						if(d�ː� == 0){
							sRet[24] = reader.GetDecimal(23).ToString().TrimEnd();
						}else{
							sRet[24] = d�ː�.ToString().TrimEnd();
						}
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						sRet[24] = "";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
					sRet[25] = reader.GetDecimal(24).ToString().Trim();
					sRet[26] = reader.GetString(25).Trim();
// MOD 2005.06.07 ���s�j�ɉ� �A�����i�R�[�h�ʑΉ��ǉ� START
//					sRet[27] = reader.GetString(26).TrimEnd();
//					sRet[28] = reader.GetString(27).TrimEnd();
					// ���Ԏw��̏ꍇ�A�Q�s�ڂ̂ݕ\��
					if (s�A�����i�b�c�P.Equals("100"))
					{
						sRet[27] = reader.GetString(27).TrimEnd();
						sRet[28] = "";
					}
					// �P�s�ڂƂQ�s�ڂ������R�[�h�̏ꍇ�A�Q�s�ڂ�\�����Ȃ�
					else if (s�A�����i�b�c�P.Equals(s�A�����i�b�c�Q))
					{
						sRet[27] = reader.GetString(26).TrimEnd();
						sRet[28] = "";
					}
					else
					{
						sRet[27] = reader.GetString(26).TrimEnd();
						sRet[28] = reader.GetString(27).TrimEnd();
					}
// MOD 2005.06.07 ���s�j�ɉ� �A�����i�R�[�h�ʑΉ��ǉ� START
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[29] = reader.GetString(28).Trim();
//					sRet[30] = reader.GetString(29).Trim();
//					sRet[31] = reader.GetString(30).Trim();
					sRet[29] = reader.GetString(28).TrimEnd(); // �i���L���P
					sRet[30] = reader.GetString(29).TrimEnd(); // �i���L���Q
					sRet[31] = reader.GetString(30).TrimEnd(); // �i���L���R
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
// MOD 2005.06.07 ���s�j�ɉ� �A�����i�ɂ���Ēl��ύX START
					// �p�[�Z���̏ꍇ�A"11"
					if (s�A�����i�b�c�P.Equals("001") || s�A�����i�b�c�P.Equals("002"))
						sRet[32] = reader.GetString(31).Trim() + "1";
					else
						sRet[32] = reader.GetString(31).Trim() + "0";
// MOD 2005.06.07 ���s�j�ɉ� �A�����i�ɂ���Ēl��ύX END
					sRet[33] = reader.GetString(32).Trim(); // ����󔭍s�ςe�f
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� START
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sRet[34] = reader.GetString(34).Trim();
					sRet[34] = reader.GetString(34).TrimEnd(); // �S���ҁi�����j
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sRet[35] = reader.GetString(35).Trim(); // ���q�l�ԍ�
// ADD 2005.06.01 ���s�j�����J �S���ҁA���q�l�ԍ��ǉ� END
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� START
					sRet[36] = reader.GetString(38).Trim();
// ADD 2005.06.08 ���s�j�ɉ� �w����敪�ǉ� END
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� START
					s�X�֔ԍ� = reader.GetString(39).Trim();
// ADD 2005.06.09 ���s�j�����J �X�֔ԍ��ǉ� END
// ADD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� START
					sRet[37] = reader.GetString(40).Trim();		//�d���b�c
					sRet[38] = reader.GetString(41).Trim();		//���X��
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
					//�Г��`�̏ꍇ�́A�o�׃e�[�u���̕��𐳂Ƃ���
					if(s���p�ҕ���X���b�c == "044")
					{
						sRet[38] = reader.GetString(50).Trim();
					}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
// ADD 2007.02.08 ���s�j���� �d���b�c�A���X���̒ǉ� END
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V START
					sRet[39] = reader.GetString(42).Trim();		//�o�׍ςe�f
// MOD 2010.11.01 ���s�j���� �o�׍ς̏ꍇ�A�o�ד����X�V END
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� START
					sRet[40] = reader.GetString(43).Trim();		//���˗���X�֔ԍ�
// MOD 2011.01.06 ���s�j���� �X�֔ԍ��̈�� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					sRet[41] = reader.GetString(44).TrimEnd();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					sRet[42] = reader.GetString(45).TrimEnd(); // �i���L���S
					sRet[43] = reader.GetString(46).TrimEnd(); // �i���L���T
					sRet[44] = reader.GetString(47).TrimEnd(); // �i���L���U
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ START
					sRet[45] = reader.GetString(48).TrimEnd(); // ���X��
// MOD 2011.12.06 ���s�j���� ���x���w�b�_���ɔ��X���E���X������ END
					iCnt++;
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (iCnt == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
				}
				else
				{
					sRet[0] = "����I��";
// DEL 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX START
//// ADD 2005.06.09 ���s�j�����J ���X��\���擾�ǉ� START
//					string styaku = Get_tyakutennasi(sUser, conn2, s�X�֔ԍ�);
//					if(styaku == "1")
//						sRet[13] = "";
//// ADD 2005.06.09 ���s�j�����J ���X��\���擾�ǉ� END
// DEL 2008.06.12 kcl)�X�{ ���X�R�[�h�������@�̕ύX END
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
					if(s���p�ҕ���X���b�c.Length == 0){
// MOD 2011.10.06 ���s�j���� �o�׃f�[�^�̈�����O�̒ǉ� START
logWriter(sUser, INF, "�o�׈���f�[�^�擾�@���p�ҕ���X���b�c��"
						+"["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sRet[11]+"]"
						+"����󔭍s��["+sRet[33]+"]�o�׍�["+sRet[39]+"]"
						);
// MOD 2011.10.06 ���s�j���� �o�׃f�[�^�̈�����O�̒ǉ� END
						return sRet;
					}
					// ���p�҂̕���̊Ǌ��X���b�c�Ɠo�^�҂̔��X�b�c�Ƃ��قȂ�ꍇ
					string s���X�b�c = sRet[14].Trim().Substring(1, 3);
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
//					if(!s���X�b�c.Equals(s���p�ҕ���X���b�c))
//					{
//						return sRet;
//					}
					//�Г��`�̏ꍇ�́A���p�҂̕���̊Ǌ��X���b�c�Ɠo�^�҂̔��X�b�c�Ƃ��قȂ��Ă��悢
					if(s���p�ҕ���X���b�c != "044")
					{
						if(!s���X�b�c.Equals(s���p�ҕ���X���b�c))
						{
							return sRet;
						}
					}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
					// �����ԍ����Ȃ��ꍇ�ɂ͎擾����
					if(sRet[11].Length == 0)
					{
						disconnect2(sUser, conn2);
						conn2 = null;
						string[] sRetInvoiceNo = Set_InvoiceNo2(sUser ,sKey, sRet, s���p�ҕ���X���b�c);
						if(sRetInvoiceNo[0].Length == 4){
//							sRet[11] = sRetInvoiceNo[1];
						}else{
							sRet[0] = sRetInvoiceNo[0];
						}
					}
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END
// MOD 2011.10.06 ���s�j���� �o�׃f�[�^�̈�����O�̒ǉ� START
logWriter(sUser, INF, "�o�׈���f�[�^�擾"
						+"["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sRet[11]+"]"
						+"����󔭍s��["+sRet[33]+"]�o�׍�["+sRet[39]+"]"
						);
// MOD 2011.10.06 ���s�j���� �o�׃f�[�^�̈�����O�̒ǉ� END
				}
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

// ADD 2005.06.09 ���s�j�����J ���X��\���擾�ǉ� START
		/*********************************************************************
		 * ���X��\���擾
		 * �����F�X�֔ԍ�
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		private string Get_tyakutennasi(string[] sUser, OracleConnection conn2, string sYuubin)
		{
			string sAri = "0";

			string cmdQuery
				= "SELECT �X�֔ԍ� \n"
				+ " FROM �b�l�P�T���X��\�� \n"
				+ " WHERE �X�֔ԍ� = '" + sYuubin + "' \n"
				+ "   AND �폜�e�f     = '0' \n";

			OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

			bool bRead = reader.Read();
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
			disposeReader(reader);
			reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
			if(bRead == true)
				sAri = "1";
			else
				sAri = "0";
			
			return sAri;
		}
// ADD 2005.06.09 ���s�j�����J ���X��\���擾�ǉ� END

		/*********************************************************************
		 * �̔Ԃ̍X�V
		 * �����F����b�c�A����b�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Get_InvoiceNo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�̔ԍX�V�J�n");
			
			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			//�g�����U�N�V�����̐ݒ�
			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
				decimal i�o�^�A��     = 0;
				decimal i�J�n���[�ԍ� = 0;
				decimal i�I�����[�ԍ� = 0;
				decimal i�ŏI���[�ԍ� = 0;
				string  s���t��       = "";
				string  s�L������     = "";
				string  s�������t     = "";

				string cmdQuery_am12 = "SELECT";
				cmdQuery_am12 += " AM12.�o�^�A�� ";
				cmdQuery_am12 += ",AM12.�J�n���[�ԍ� ";
				cmdQuery_am12 += ",AM12.�I�����[�ԍ� ";
				cmdQuery_am12 += ",AM12.�ŏI���[�ԍ� ";
				cmdQuery_am12 += ",AM12.���t�� ";
				cmdQuery_am12 += ",AM12.�L������ ";
				cmdQuery_am12 += ",TO_CHAR(SYSDATE,'YYYYMMDD') \n";
				cmdQuery_am12 += " FROM �`�l�P�Q�����̔� AM12 \n";
				cmdQuery_am12 += " WHERE AM12.����b�c = '" + sKey[0] + "' \n";
				cmdQuery_am12 += " AND AM12.����b�c = '" + sKey[1] + "' \n";
				cmdQuery_am12 += " AND AM12.�����敪 = '" + sKey[2] + "' \n";
				cmdQuery_am12 += " AND AM12.�폜�e�f = '0' \n";
				cmdQuery_am12 += " FOR UPDATE \n";

				OracleDataReader reader_am12 = CmdSelect(sUser, conn2, cmdQuery_am12);
				int intCnt_am12 = 0;
				sRet[1] = "";
				if (reader_am12.Read())
				{
					i�o�^�A��     = reader_am12.GetDecimal(0);
					i�J�n���[�ԍ� = reader_am12.GetDecimal(1);
					i�I�����[�ԍ� = reader_am12.GetDecimal(2);
					i�ŏI���[�ԍ� = reader_am12.GetDecimal(3);
					s���t��       = reader_am12.GetString(4).Trim();
					s�L������     = reader_am12.GetString(5).Trim();
					s�������t     = reader_am12.GetString(6).Trim();
					intCnt_am12++;

					if (i�ŏI���[�ԍ� < i�I�����[�ԍ� && int.Parse(s�L������) >= int.Parse(s�������t))
					{
						//�����ԍ��̃Z�b�g
						sRet[1] = (i�ŏI���[�ԍ� + 1).ToString();
					}
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader_am12);
				reader_am12 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (sRet[1].Length == 0)
				{
					//�`�l�P�Q�����̔ԂɃL�[�����݂��Ȃ��A�܂���
					//�ŏI�ԍ� >= �I���ԍ��A�܂���
					//�L������ <  �����̎�
					decimal i�ő�A��   = 0;
					decimal i�J�n�ԍ�   = 0;
					decimal i�ŏI�ԍ�   = 0;
					decimal i�I���ԍ�   = 0;
					decimal i���t����   = 0;
					decimal i�L������   = 0;
					decimal i�L�������N = 0;
					decimal i�L�������� = 0;
					decimal i�L�������� = 0;

					//�̔ԊǗ����V�K���[�ԍ��g���擾
					string cmdQuery_am10 = "SELECT";
					cmdQuery_am10 += " AM10.�ő�A�� ";
					cmdQuery_am10 += ",AM10.�o�^�A�� ";
					cmdQuery_am10 += ",AM10.�ŏI���[�ԍ� ";
					cmdQuery_am10 += ",AM11.�I�����[�ԍ� ";
					cmdQuery_am10 += ",AM10.���t���� ";
					cmdQuery_am10 += ",AM10.�L������ ";
					cmdQuery_am10 += ",TO_CHAR(SYSDATE,'YYYYMMDD') \n";
					cmdQuery_am10 += "FROM �`�l�P�O�̔ԊǗ� AM10 ";
					cmdQuery_am10 += ",�`�l�P�P�����ԍ� AM11 \n";
					cmdQuery_am10 += " WHERE AM10.�̔ԋ敪 = '" + sKey[2] + "' \n";
					//cmdQuery_am10 += "   AND AM10.�o�^�A��       =  " + i�o�^�A��;
					cmdQuery_am10 += " AND AM10.�̔ԋ敪 = AM11.�����敪 \n";
					cmdQuery_am10 += " AND AM10.�o�^�A�� = AM11.�o�^�A�� \n";
					cmdQuery_am10 += " AND AM10.�폜�e�f = '0' \n";
					cmdQuery_am10 += " FOR UPDATE \n";

					OracleDataReader reader_am10 = CmdSelect(sUser, conn2, cmdQuery_am10);
					int intCnt_am10 = 0;
					if (reader_am10.Read())
					{
						i�ő�A��     = reader_am10.GetDecimal(0);
						i�o�^�A��     = reader_am10.GetDecimal(1);
						i�ŏI�ԍ�     = reader_am10.GetDecimal(2);
						i�I���ԍ�     = reader_am10.GetDecimal(3);
						i���t����     = reader_am10.GetDecimal(4);
						i�L������     = reader_am10.GetDecimal(5);
						s�������t     = reader_am10.GetString(6);

						//�����̔ԍX�V���̎擾
						i�J�n���[�ԍ� = i�ŏI�ԍ� + 1;
						i�I�����[�ԍ� = i�ŏI�ԍ� + i���t����;
						i�ŏI���[�ԍ� = i�J�n���[�ԍ�;
						s���t��       = s�������t;
						i�L�������N   = int.Parse(s���t��.Substring(0, 4));
						i�L��������   = int.Parse(s���t��.Substring(4, 2)) + i�L������ - 1;
						if (i�L�������� > 12)
						{
							i�L�������N++;
							i�L�������� = i�L�������� - 12;
						}
						i�L��������   = System.DateTime.DaysInMonth(decimal.ToInt32(i�L�������N), decimal.ToInt32(i�L��������));
						s�L������     = i�L�������N.ToString() + i�L��������.ToString().PadLeft(2, '0') + i�L��������.ToString().PadLeft(2, '0');

						//�̔ԊǗ��X�V���̎擾
						i�ŏI�ԍ�     = i�I�����[�ԍ�;

						sRet[1] = i�ŏI���[�ԍ�.ToString();
						intCnt_am10++;
					}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
					disposeReader(reader_am10);
					reader_am10 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
					if (intCnt_am10 == 0)
					{
						//�Y���f�[�^���Ȃ��ꍇ�̓G���[
						throw new Exception("�Y���f�[�^������܂���");
					}
					if (i�ŏI�ԍ� > i�I���ԍ�)
					{
						i�o�^�A��++;
						if (i�o�^�A�� > i�ő�A��)
						{
							i�o�^�A�� = 1;
						}
						//�����ԍ����V�K���[�ԍ��g���擾
						string cmdQuery_am11 = "SELECT";
						cmdQuery_am11 += " AM11.�J�n���[�ԍ� \n";
						cmdQuery_am11 += " FROM �`�l�P�P�����ԍ� AM11 \n";
						cmdQuery_am11 += " WHERE AM11.�����敪 = '" + sKey[2] + "' \n";
						cmdQuery_am11 += " AND AM11.�o�^�A�� =  " + i�o�^�A�� + " \n";
						cmdQuery_am11 += " AND AM11.�폜�e�f = '0' \n";
						cmdQuery_am11 += " FOR UPDATE \n";

						OracleDataReader reader_am11 = CmdSelect(sUser, conn2, cmdQuery_am11);
						int intCnt_am11 = 0;
						if (reader_am11.Read())
						{
							i�J�n�ԍ�     = reader_am11.GetDecimal(0);
							//�̔ԊǗ��X�V���̎擾
							i�ŏI�ԍ�     = i�J�n�ԍ� + i���t���� - 1;
							//�����̔ԍX�V���̎擾
							i�J�n���[�ԍ� = i�J�n�ԍ�;
							i�I�����[�ԍ� = i�ŏI�ԍ�;
							i�ŏI���[�ԍ� = i�J�n���[�ԍ�;

							sRet[1] = i�ŏI���[�ԍ�.ToString();
							intCnt_am11++;
						}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
						disposeReader(reader_am11);
						reader_am11 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
						if (intCnt_am11 == 0)
						{
							//�Y���f�[�^���Ȃ��ꍇ�̓G���[
							throw new Exception("�Y���f�[�^������܂���");
						}
					}
					// �̔ԊǗ��̍X�V
					string updQuery_am10 = "UPDATE �`�l�P�O�̔ԊǗ� \n";
					updQuery_am10 += " SET �o�^�A�� = " + i�o�^�A��;
					updQuery_am10 += ", �ŏI���[�ԍ� = " + i�ŏI�ԍ�;
					updQuery_am10 += ", �X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "; // �X�V����
					updQuery_am10 += ", �X�V�� = '" + sKey[3] + "' \n";                   // �X�V��
					updQuery_am10 += " WHERE �̔ԋ敪 = '" + sKey[2] + "' \n";

					CmdUpdate(sUser, conn2, updQuery_am10);
				}

				string updQuery_am12 = "";
				if (intCnt_am12 == 0)
				{
					// �����̔Ԃ̒ǉ�
					updQuery_am12  = "INSERT INTO �`�l�P�Q�����̔� \n";
					updQuery_am12 += " VALUES ('" + sKey[0] + "' ";
					updQuery_am12 +=         ",'" + sKey[1] + "' ";
					updQuery_am12 +=         ",'" + sKey[2] + "' ";
					updQuery_am12 +=         ", " + i�o�^�A��;
					updQuery_am12 +=         ", " + i�J�n���[�ԍ�;
					updQuery_am12 +=         ", " + i�I�����[�ԍ�;
					updQuery_am12 +=         ", " + i�ŏI���[�ԍ�;
					updQuery_am12 +=         ",'" + s���t�� + "' ";
					updQuery_am12 +=         ",'" + s�L������ + "' ";
					updQuery_am12 +=         ",'0' ";
					updQuery_am12 +=         ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=         ",'�o�דo�^' ";
					updQuery_am12 +=         ",'" + sKey[3] + "' ";
					updQuery_am12 +=         ", TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=         ",'�o�דo�^' ";
					updQuery_am12 +=         ",'" + sKey[3] + "' ";
					updQuery_am12 += " ) ";
				}
				else
				{
					// �����̔Ԃ̍X�V
					updQuery_am12  = "UPDATE �`�l�P�Q�����̔� \n";
					updQuery_am12 += " SET �o�^�A�� =  " + i�o�^�A��;
					updQuery_am12 +=      ", �J�n���[�ԍ� =  " + i�J�n���[�ԍ�;
					updQuery_am12 +=      ", �I�����[�ԍ� =  " + i�I�����[�ԍ�;
					updQuery_am12 +=      ", �ŏI���[�ԍ� =  " + sRet[1];
					updQuery_am12 +=      ", ���t�� = '" + s���t�� + "'";
					updQuery_am12 +=      ", �L������ = '" + s�L������ + "'";
					updQuery_am12 +=      ", �X�V���� =   TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";
					updQuery_am12 +=      ", �X�V�o�f = '�o�דo�^' ";
					updQuery_am12 +=      ", �X�V�� = '" + sKey[3] + "' \n";
					updQuery_am12 += " WHERE ����b�c = '" + sKey[0] + "' \n";
					updQuery_am12 +=   " AND ����b�c = '" + sKey[1] + "' \n";
					updQuery_am12 +=   " AND �����敪 = '" + sKey[2] + "' \n";
				}
				CmdUpdate(sUser, conn2, updQuery_am12);
				tran.Commit();
				sRet[0] = "����I��";
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}

		/*********************************************************************
		 * ����󔭍s�ςe�f�̍X�V
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n�A�����ԍ��A�X�V��
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Set_InvoiceNo(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���s�ςe�f�X�V�J�n");

			OracleConnection conn2 = null;
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
//			string[] sRet = new string[1];
			string[] sRet = new string[2]{"",""};
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try
			{
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� START
				StringBuilder sbQuery = new StringBuilder(1024);
				string s���X�b�c = "";
				string s���X��   = "";
				sbQuery.Append("SELECT NVL(CM14.�X���b�c, ' ') \n");
				sbQuery.Append(", NVL(CM10.�X����, ' ') \n");
				sbQuery.Append(" FROM �b�l�O�Q���� CM02 \n");
				sbQuery.Append(" LEFT JOIN �b�l�P�S�X�֔ԍ� CM14 \n");
				sbQuery.Append(" ON CM02.�X�֔ԍ� = CM14.�X�֔ԍ� \n");
				sbQuery.Append(" LEFT JOIN �b�l�P�O�X�� CM10 \n");
				sbQuery.Append(" ON CM14.�X���b�c = CM10.�X���b�c \n");
				sbQuery.Append(" WHERE CM02.����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND CM02.����b�c = '" + sKey[1] + "' \n");
				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s���X�b�c = reader.GetString(0).Trim();
					s���X��   = reader.GetString(1).Trim();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� END
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� START
//				if(s���X�b�c == "030"){
//					sbQuery = new StringBuilder(1024);
//					sbQuery.Append("SELECT NVL(CM14.�X���b�c, ' ') \n");
//					sbQuery.Append(", NVL(CM10.�X����, ' ') \n");
//					sbQuery.Append(" FROM \"�r�s�O�P�o�׃W���[�i��\" ST01 \n");
//					sbQuery.Append(" LEFT JOIN �r�l�O�P�ב��l SM01 \n");
//					sbQuery.Append(" ON  ST01.����b�c = SM01.����b�c \n");
//					sbQuery.Append(" AND ST01.����b�c = SM01.����b�c \n");
//					sbQuery.Append(" AND ST01.�ב��l�b�c = SM01.�ב��l�b�c \n");
//					sbQuery.Append(" LEFT JOIN �b�l�P�S�X�֔ԍ� CM14 \n");
//					sbQuery.Append(" ON SM01.�X�֔ԍ� = CM14.�X�֔ԍ� \n");
//					sbQuery.Append(" LEFT JOIN �b�l�P�O�X�� CM10 \n");
//					sbQuery.Append(" ON CM14.�X���b�c = CM10.�X���b�c \n");
//					sbQuery.Append(" WHERE ST01.����b�c = '" + sKey[0] + "' \n");
//					sbQuery.Append(" AND ST01.����b�c = '" + sKey[1] + "' \n");
//					sbQuery.Append(" AND ST01.�o�^�� = '" + sKey[2] + "' \n");
//					sbQuery.Append(" AND ST01.�W���[�i���m�n = '" + sKey[3] + "' \n");
//					reader = CmdSelect(sUser, conn2, sbQuery);
//					if(reader.Read()){
//						s���X�b�c = reader.GetString(0).Trim();
//						s���X��   = reader.GetString(1).Trim();
//					}
//					disposeReader(reader);
//					reader = null;
//					sbQuery = null;
//				}
//�ۗ� MOD 2010.07.21 ���s�j���� ���R�[�l�Ή� END
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
				// �����ԍ��`�F�b�N
				sbQuery = new StringBuilder(1024);
				string s�����ԍ� = "";
				sbQuery.Append("SELECT �����ԍ� \n");
				sbQuery.Append(" FROM  \"�r�s�O�P�o�׃W���[�i��\" \n");
				sbQuery.Append(" WHERE ����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ����b�c = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND �o�^��   = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND \"�W���[�i���m�n\" = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND �폜�e�f = '0' \n");
				sbQuery.Append(" FOR UPDATE \n");
				reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s�����ԍ� = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
				if(s�����ԍ�.Length > 0){
					// �قȂ鑗���ԍ����㏑�����悤�Ƃ����ꍇ
					if(s�����ԍ� != sKey[4]){
						tran.Commit();
						sRet[0] = "�G���[�F���̒[���ň�����������͈���ςł�\n"
								+ "["+s�����ԍ�.Substring(4)+"]";
						sRet[1] = s�����ԍ�;
logWriter(sUser, INF, "�����ԍ��X�V��["+sRet[1]+"]"
						+ " ["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sKey[4]+"]");
						return sRet;
					}
				}

// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END
				// �o�׃W���[�i���̍X�V
				string cmdQuery  = "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n";
				cmdQuery += " SET �����ԍ� = '"  + sKey[4] + "' ";                     // �����ԍ�
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
				cmdQuery +=     ",�����O�P = TO_CHAR(SYSDATE,'MMDDHH24MISS') \n"; // ����������������b
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END
				cmdQuery +=     ",����󔭍s�ςe�f = '1' ";
				cmdQuery +=     ",��� = DECODE(���,'01','02','02','02',���) ";
				cmdQuery +=     ",�ڍ׏�� = DECODE(���,'01','  ','02','  ',�ڍ׏��) ";
				cmdQuery +=     ",�X�V���� =   TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') ";    // �X�V����
				cmdQuery +=     ",�X�V�o�f = '�o�דo�^' ";                               // �X�V�o�f
				cmdQuery +=     ",�X�V�� = '" + sKey[5] + "' \n";                        // �X�V��
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� START
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
//				if(s���X�b�c.Length > 0)
//				{
//					cmdQuery += ",���X�b�c = '" + s���X�b�c + "' \n";
//				}
//				if(s���X��.Length > 0){
//					cmdQuery += ",���X�� = '"   + s���X��   + "' \n";
//				}
				//�Г��`�̏ꍇ�́A�o�׃e�[�u���̕��𐳂Ƃ���
				if(sKey[0].Substring(0,2).ToUpper() != "FK")
				{
					if(s���X�b�c.Length > 0)
					{
						cmdQuery += ",���X�b�c = '" + s���X�b�c + "' \n";
					}
					if(s���X��.Length > 0)
					{
						cmdQuery += ",���X�� = '"   + s���X��   + "' \n";
					}
				}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END
// MOD 2010.06.03 ���s�j���� �X�֔ԍ��}�X�^�̓X���ύX���̑Ή� END
				cmdQuery += " WHERE ����b�c       = '" + sKey[0] + "' \n";
				cmdQuery +=   " AND ����b�c       = '" + sKey[1] + "' \n";
				cmdQuery +=   " AND �o�^��         = '" + sKey[2] + "' \n";
				cmdQuery +=   " AND �W���[�i���m�n = '" + sKey[3] + "' \n";
				cmdQuery +=   " AND �폜�e�f       = '0' \n";
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� START
logWriter(sUser, INF, "���s�ςe�f�X�V["+sKey[1]+"]["+sKey[2]+"]["+sKey[3]+"]:["+sKey[4]+"]");
// MOD 2010.06.18 ���s�j���� �o�׃f�[�^�̎Q�ƁE�ǉ��E�X�V�E�폜���O�̒ǉ� END

				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "����I��";
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ START
		/*********************************************************************
		 * �����ԍ��X�V
		 * �����F����b�c�A����b�c�A�o�^���A�W���[�i���m�n�A�����ԍ��A�X�V��
		 * �@�@�@����f�[�^�A���p�ҕ���X���b�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
//		[WebMethod]
		private String[] Set_InvoiceNo2(string[] sUser, string[] sKey, string[] sPrintData, string sTensyo)
		{
			logWriter(sUser, INF, "�����ԍ��X�V�Q�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2]{"",""};

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null){
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			try{
				StringBuilder sbQuery = new StringBuilder(1024);
				string s�����ԍ� = "";
				sbQuery.Append("SELECT �����ԍ� \n");
				sbQuery.Append(" FROM  \"�r�s�O�P�o�׃W���[�i��\" \n");
				sbQuery.Append(" WHERE ����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ����b�c = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND �o�^��   = '" + sKey[2] + "' \n");
				sbQuery.Append(" AND \"�W���[�i���m�n\" = '" + sKey[3] + "' \n");
				sbQuery.Append(" AND �폜�e�f = '0' \n");
				sbQuery.Append(" FOR UPDATE \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);
				if(reader.Read()){
					s�����ԍ� = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				sbQuery = null;
				if(s�����ԍ�.Length > 0){
					tran.Commit();
					sRet[0] = "�̔ԍς�";
					sRet[1] = s�����ԍ�;
logWriter(sUser, INF, "�����ԍ��X�V�Q�@�����ԍ��X�V��["+s�����ԍ�+"]");
					return sRet;
				}
				// �����ԍ��`�F�b�N
				String[] sGetKey = new string[4];
//				sGetKey[1] = gs����b�c;
//				sGetKey[1] = gs����b�c;
//				sGetKey[0] = sUser[0];
				sGetKey[0] = sKey[0];
//				sGetKey[1] = sKey[1];
				sGetKey[1] = sTensyo; // ���p�ҕ���X���b�c
				sGetKey[2] = sPrintData[32]; //�����敪 + "0" or "1"
				if(sPrintData[14].Substring(1, 3) == "047"){
					sGetKey[2] = sPrintData[32].Substring(0,1) + "G"; //�����敪 + "G"
				}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� START
				if(sTensyo == "044")
				{
					sGetKey[2] = sPrintData[32].Substring(0,1) + "F"; //�����敪 + "F"
				}
// MOD 2016.04.15 bevas) ���{ �Г��`�[�@�\�ǉ��Ή� END

//				sGetKey[3] = gs���p�҂b�c;
				sGetKey[3] = sUser[1];
				String[] sGetData = this.Get_InvoiceNo(sUser, sGetKey);
				if(sGetData[0].Length != 4){
					tran.Commit();
					sRet[0] = sGetData[0];
					return sRet;
				}
				//�����ԍ��̃Z�b�g
				sPrintData[11] = sGetData[1].PadLeft(14, '0');
				//�`�F�b�N�f�W�b�g�i�V�Ŋ������]��j�̕t��
				sPrintData[11] = sPrintData[11] + (long.Parse(sPrintData[11]) % 7).ToString();

				// �o�׃W���[�i���̍X�V
				string cmdQuery  = "UPDATE \"�r�s�O�P�o�׃W���[�i��\" \n";
				cmdQuery += " SET �����ԍ� = '"  + sPrintData[11] + "' ";                     // �����ԍ�
				cmdQuery += " WHERE ����b�c       = '" + sKey[0] + "' \n";
				cmdQuery +=   " AND ����b�c       = '" + sKey[1] + "' \n";
				cmdQuery +=   " AND �o�^��         = '" + sKey[2] + "' \n";
				cmdQuery +=   " AND �W���[�i���m�n = '" + sKey[3] + "' \n";
				cmdQuery +=   " AND �폜�e�f       = '0' \n";

				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "����I��";
//				sRet[1] = sPrintData[11];
logWriter(sUser, INF, "�����ԍ��X�V�Q�@�����ԍ��X�V["+sPrintData[11]+"]");
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// MOD 2011.03.25 ���s�j���� �����ԍ��̏㏑���h�~ END

		/*********************************************************************
		 * �˗������f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�ב��l�b�c�A�J�i���́A�d�b�ԍ�...
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ConsignorPrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�˗������f�[�^�擾�J�n");
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� START
			string s����b�c     = (sKey.Length >  0) ? sKey[ 0] : "";
			string s����b�c     = (sKey.Length >  1) ? sKey[ 1] : "";
			string s�˗���J�i   = (sKey.Length >  2) ? sKey[ 2] : "";
			if(s�˗���J�i == null) s�˗���J�i = "";
			string s�˗���R�[�h = (sKey.Length >  3) ? sKey[ 3] : "";
			string s�˗��喼�O   = (sKey.Length >  4) ? sKey[ 4] : "";
			string s������b�c   = (sKey.Length >  5) ? sKey[ 5] : "";
			string s���ۂb�c     = (sKey.Length >  6) ? sKey[ 6] : "";
			string s�K�w���X�g�P = (sKey.Length >  7) ? sKey[ 7] : "2";
			string s�\�[�g�����P = (sKey.Length >  8) ? sKey[ 8] : "0";
			string s�K�w���X�g�Q = (sKey.Length >  9) ? sKey[ 9] : "0";
			string s�\�[�g�����Q = (sKey.Length > 10) ? sKey[10] : "0";

			int i�K�w���X�g�P = 2;
			int i�\�[�g�����P = 0;
			int i�K�w���X�g�Q = 0;
			int i�\�[�g�����Q = 0;
			try{
				i�K�w���X�g�P = int.Parse(s�K�w���X�g�P);
				i�\�[�g�����P = int.Parse(s�\�[�g�����P);
				i�K�w���X�g�Q = int.Parse(s�K�w���X�g�Q);
				i�\�[�g�����Q = int.Parse(s�\�[�g�����Q);
			}catch(Exception){
				;
			}
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� END

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT");
				sbQuery.Append(" SM01.�ב��l�b�c ");
				sbQuery.Append(",SM01.�J�i���� ");
				sbQuery.Append(",SM01.�d�b�ԍ��P ");
				sbQuery.Append(",SM01.�d�b�ԍ��Q ");
				sbQuery.Append(",SM01.�d�b�ԍ��R ");
				sbQuery.Append(",SM01.�X�֔ԍ� ");
				sbQuery.Append(",SM01.�Z���P ");
				sbQuery.Append(",SM01.�Z���Q ");
				sbQuery.Append(",SM01.�Z���R ");
				sbQuery.Append(",SM01.���O�P ");
				sbQuery.Append(",SM01.���O�Q ");
				sbQuery.Append(",SM01.�d�� ");
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� START
//				sbQuery.Append(",NVL(SM04.���Ӑ�b�c, ' ') ");
//				sbQuery.Append(",NVL(SM04.���Ӑ敔�ۂb�c, ' ') ");
				sbQuery.Append(",NVL(SM01.���Ӑ�b�c, ' ') ");
				sbQuery.Append(",NVL(SM01.���Ӑ敔�ۂb�c, ' ') ");
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� END
				sbQuery.Append(",NVL(SM04.���Ӑ敔�ۖ�, ' ') \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(",SM01.�ː� \n");
				sbQuery.Append(",NVL(CM01.�ۗ�����e�f,'0') \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				sbQuery.Append(" FROM \"�r�l�O�P�ב��l\" SM01 \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(" LEFT JOIN �b�l�O�P��� CM01 \n");
				sbQuery.Append(" ON SM01.����b�c = CM01.����b�c \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
				sbQuery.Append(" LEFT JOIN \"�b�l�O�Q����\" CM02 \n");
				sbQuery.Append(" ON SM01.����b�c = CM02.����b�c ");
				sbQuery.Append("AND SM01.����b�c = CM02.����b�c ");
				sbQuery.Append("AND CM02.�폜�e�f = '0' \n");
				sbQuery.Append(" LEFT JOIN \"�r�l�O�S������\" SM04 \n");
				sbQuery.Append(" ON CM02.�X�֔ԍ� = SM04.�X�֔ԍ� ");
				sbQuery.Append("AND SM01.���Ӑ�b�c = SM04.���Ӑ�b�c ");
				sbQuery.Append("AND SM01.���Ӑ敔�ۂb�c = SM04.���Ӑ敔�ۂb�c ");
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
				sbQuery.Append("AND SM01.����b�c = SM04.����b�c ");
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
				sbQuery.Append("AND SM04.�폜�e�f = '0' \n");
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� START
//				sbQuery.Append(" WHERE SM01.����b�c = '" + sKey[0] + "' \n");
//				sbQuery.Append(" AND SM01.����b�c = '" + sKey[1] + "' \n");
//				sbQuery.Append(" AND SM01.�폜�e�f = '0' \n");
//				sbQuery.Append(" ORDER BY SM01.�ב��l�b�c \n");
				sbQuery.Append(" WHERE SM01.����b�c = '" + s����b�c + "' \n");
				sbQuery.Append(" AND SM01.����b�c = '" + s����b�c + "' \n");

				if(s�˗���J�i.Length > 0){
					sbQuery.Append(" AND SM01.�J�i���� LIKE '"+ s�˗���J�i + "%' \n");
				}
				if(s�˗���R�[�h.Length > 0){
					sbQuery.Append(" AND SM01.�ב��l�b�c LIKE '"+ s�˗���R�[�h + "%' \n");
				}
				if(s�˗��喼�O.Length > 0){
					sbQuery.Append(" AND SM01.���O�P LIKE '%"+ s�˗��喼�O + "%' \n");
				}
				if(s������b�c.Length > 0){
					sbQuery.Append(" AND SM01.���Ӑ�b�c = '"+ s������b�c + "' \n");
					if(s���ۂb�c.Length > 0){
						sbQuery.Append(" AND SM01.���Ӑ敔�ۂb�c = '"+ s���ۂb�c + "' \n");
					}else{
						sbQuery.Append(" AND SM01.���Ӑ敔�ۂb�c = ' ' \n");
					}
				}
				sbQuery.Append(  "AND SM01.�폜�e�f = '0' \n");

				sbQuery.Append(" ORDER BY \n");
				switch(i�K�w���X�g�P){
				case 1:
					sbQuery.Append(" SM01.�J�i���� ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 2:
					sbQuery.Append(" SM01.�ב��l�b�c");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 3:
//					sbQuery.Append(" SM01.���Ӑ�b�c ");
//					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.���Ӑ敔�ۂb�c ");
					sbQuery.Append(" NVL(SM04.���Ӑ敔�ۖ�,SM01.���Ӑ�b�c || SM01.���Ӑ敔�ۂb�c) ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 4:
					sbQuery.Append(" SM01.���O�P ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.���O�Q ");
//					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 5:
					sbQuery.Append(" SM01.�d�b�ԍ��P ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.�d�b�ԍ��Q ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.�d�b�ԍ��R ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 6:
					sbQuery.Append(" SM01.�o�^���� ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				case 7:
					sbQuery.Append(" SM01.�X�V���� ");
					if(i�\�[�g�����P == 1) sbQuery.Append(" DESC ");
					break;
				}
				if(i�K�w���X�g�P != 0 && i�K�w���X�g�Q != 0){
					sbQuery.Append(",");
				}
				switch(i�K�w���X�g�Q){
				case 1:
					sbQuery.Append(" SM01.�J�i���� ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 2:
					sbQuery.Append(" SM01.�ב��l�b�c");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 3:
//					sbQuery.Append(" SM01.���Ӑ�b�c ");
//					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.���Ӑ敔�ۂb�c ");
					sbQuery.Append(" NVL(SM04.���Ӑ敔�ۖ�,SM01.���Ӑ�b�c || SM01.���Ӑ敔�ۂb�c) ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 4:
					sbQuery.Append(" SM01.���O�P ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
//					sbQuery.Append(", SM01.���O�Q ");
//					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 5:
					sbQuery.Append(" SM01.�d�b�ԍ��P ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.�d�b�ԍ��Q ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					sbQuery.Append(", SM01.�d�b�ԍ��R ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 6:
					sbQuery.Append(" SM01.�o�^���� ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				case 7:
					sbQuery.Append(" SM01.�X�V���� ");
					if(i�\�[�g�����Q == 1) sbQuery.Append(" DESC ");
					break;
				}
				if(i�K�w���X�g�P == 0 && i�K�w���X�g�Q == 0){
					sbQuery.Append(" SM01.���O�P \n");
				}
				if(i�K�w���X�g�P != 2 && i�K�w���X�g�Q != 2){
					sbQuery.Append(", SM01.�ב��l�b�c \n");
				}
// MOD 2010.09.08 ���s�j���� �b�r�u�o�͋@�\�̒ǉ� END

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//					string[] sData = new string[16];
					string[] sData = new string[18];
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // �J�i����
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
// ADD 2005.09.02 ���s�j�����J Trim�͂��� START
//					sData[5]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(0,3);
//					sData[6]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(3,4);
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// ADD 2005.09.02 ���s�j�����J Trim�͂��� START
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); // �Z���P
					sData[8]  = reader.GetString(7).TrimEnd(); // �Z���Q
					sData[9]  = reader.GetString(8).TrimEnd(); // �Z���R
					sData[10] = reader.GetString(9).TrimEnd(); // ���O�P
					sData[11] = reader.GetString(10).TrimEnd();// ���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[12] = reader.GetDecimal(11).ToString().Trim();
					sData[13] = reader.GetString(12).Trim();
					sData[14] = reader.GetString(13).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[15] = reader.GetString(14).Trim();
					sData[15] = reader.GetString(14).TrimEnd(); // ���Ӑ敔�ۖ�
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					sData[16] = reader.GetDecimal(15).ToString().TrimEnd(); // �ː�
					sData[17] = reader.GetString(16).TrimEnd(); // �d�ʓ��͐���
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * �͂������f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�׎�l�b�c�A�J�i���́A�d�b�ԍ�...
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ConsigneePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�͂������f�[�^�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			StringBuilder sbQuery = new StringBuilder(1024);
			try
			{
				sbQuery.Append("SELECT");
				sbQuery.Append(" �׎�l�b�c ");
				sbQuery.Append(",�J�i���� ");
				sbQuery.Append(",�d�b�ԍ��P ");
				sbQuery.Append(",�d�b�ԍ��Q ");
				sbQuery.Append(",�d�b�ԍ��R ");
				sbQuery.Append(",�X�֔ԍ� ");
				sbQuery.Append(",�Z���P ");
				sbQuery.Append(",�Z���Q ");
				sbQuery.Append(",�Z���R ");
				sbQuery.Append(",���O�P ");
				sbQuery.Append(",���O�Q ");
				sbQuery.Append(",����v \n");
				sbQuery.Append(" FROM \"�r�l�O�Q�׎�l\" \n");
				sbQuery.Append(" WHERE ����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ����b�c = '" + sKey[1] + "' \n");
				sbQuery.Append(" AND �폜�e�f = '0' \n");
				sbQuery.Append(" ORDER BY �׎�l�b�c \n");

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
					string[] sData = new string[13];
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // �J�i����
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
// ADD 2005.09.02 ���s�j�����J Trim�͂��� START
//					sData[5]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(0,3);
//					sData[6]  = reader.GetString(5).Trim().PadRight(7, '0').Substring(3,4);
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// ADD 2005.09.02 ���s�j�����J Trim�͂��� START
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); // �Z���P
					sData[8]  = reader.GetString(7).TrimEnd(); // �Z���Q
					sData[9]  = reader.GetString(8).TrimEnd(); // �Z���R
					sData[10] = reader.GetString(9).TrimEnd(); // ���O�P
					sData[11] = reader.GetString(10).TrimEnd();// ���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[12] = reader.GetString(11).Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * �L������f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�L���b�c�A�L��
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_NotePrintData(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�L������f�[�^�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			try
			{
				//string cmdQuery = "SELECT";
				//cmdQuery += " �L���b�c ";
				//cmdQuery += ",�L�� ";
				//cmdQuery +=  "FROM \"�r�l�O�R�L��\" ";
				//cmdQuery += "WHERE ����b�c   = '" + sKey[0] + "' ";
				//cmdQuery +=   "AND ����b�c   = '" + sKey[1] + "' ";
				//cmdQuery +=   "AND �폜�e�f   = '0' ";
				//cmdQuery += "ORDER BY �L���b�c \n";
				//�A���w���̎擾
				System.Text.StringBuilder cmdQuery_y = new System.Text.StringBuilder(256);
				cmdQuery_y.Append("SELECT ");
				cmdQuery_y.Append(" SM03_1.�L���b�c ");
				cmdQuery_y.Append(",SM03_1.�L�� ");
				cmdQuery_y.Append(",NVL(SM03_2.�L���b�c, ' ') ");
				cmdQuery_y.Append(",NVL(SM03_2.�L��, ' ') ");
				cmdQuery_y.Append(" FROM \"�r�l�O�R�L��\" SM03_1 ");
				cmdQuery_y.Append(" LEFT JOIN \"�r�l�O�R�L��\" SM03_2 ");
				cmdQuery_y.Append(       " ON SM03_1.����b�c = SM03_2.����b�c ");
				cmdQuery_y.Append(      " AND SM03_1.�L���b�c = SM03_2.����b�c ");
				cmdQuery_y.Append(      " AND '0'             = SM03_2.�폜�e�f ");
				cmdQuery_y.Append("WHERE SM03_1.����b�c   = 'yusoshohin' ");
				cmdQuery_y.Append(  "AND SM03_1.����b�c   = '0000' ");
				cmdQuery_y.Append(  "AND SM03_1.�폜�e�f   = '0' ");
				cmdQuery_y.Append("ORDER BY SM03_1.�L���b�c,SM03_2.�L���b�c \n");
				OracleDataReader reader_y = CmdSelect(sUser, conn2, cmdQuery_y);

				//�i���L���̎擾
				System.Text.StringBuilder cmdQuery_h = new System.Text.StringBuilder(256);
				cmdQuery_h.Append("SELECT ");
				cmdQuery_h.Append(" �L���b�c ");
				cmdQuery_h.Append(",�L�� ");
				cmdQuery_h.Append(" FROM \"�r�l�O�R�L��\" ");
				cmdQuery_h.Append("WHERE ����b�c   = '" + sKey[0] + "' ");
				cmdQuery_h.Append(  "AND ����b�c   = '" + sKey[1] + "' ");
				cmdQuery_h.Append(  "AND �폜�e�f   = '0' ");
				cmdQuery_h.Append("ORDER BY �L���b�c \n");
				OracleDataReader reader_h = CmdSelect(sUser, conn2, cmdQuery_h);

				bool b�A���w�� = true;
				bool b�i���L�� = true;
				string s�e�L�� = "";
				while (true)
				{
					if (b�A���w��) b�A���w�� = reader_y.Read();
					if (b�i���L��) b�i���L�� = reader_h.Read();

					string[] sData = new string[4];
					if (b�A���w��)
					{
						sData[0]  = reader_y.GetString(0).TrimEnd();
						sData[1]  = reader_y.GetString(1).TrimEnd();
					}
					else
					{
						sData[0] = "";
						sData[1] = "";
					}
					if (b�A���w�� && !sData[0].Equals(s�e�L��))
					{
						if (b�i���L��)
						{
							sData[2]  = reader_h.GetString(0).TrimEnd();
							sData[3]  = reader_h.GetString(1).TrimEnd();
// DEL 2005.06.01 ���s�j�ɉ� �A�����i�d�l�ύX START
//							if (b�i���L��) b�i���L�� = reader_h.Read();
// DEL 2005.06.01 ���s�j�ɉ� �A�����i�d�l�ύX END
						}
						else
						{
							sData[2] = "";
							sData[3] = "";
						}
						s�e�L�� = sData[0];
						alRet.Add(sData);
// MOD 2005.06.01 ���s�j�ɉ� �A�����i�d�l�ύX START
//						sData = new string[4];
//						sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
//						sData[1]  = "�@�@�@" + reader_y.GetString(3).TrimEnd();
						if (!reader_y.GetString(2).TrimEnd().Equals(""))
						{
							sData = new string[4];
							if (b�i���L��) b�i���L�� = reader_h.Read();
							sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
							sData[1]  = "�@�@�@" + reader_y.GetString(3).TrimEnd();
						}
						else
						{
							continue;
						}
// MOD 2005.06.01 ���s�j�ɉ� �A�����i�d�l�ύX START
					}
					else
					{
						if (b�A���w��)
						{
							sData[0]  = "  " + reader_y.GetString(2).TrimEnd();
							sData[1]  = "�@�@�@" + reader_y.GetString(3).TrimEnd();
						}
					}

					if (b�i���L��)
					{
						sData[2]  = reader_h.GetString(0).TrimEnd();
						sData[3]  = reader_h.GetString(1).TrimEnd();
					}
					else
					{
						sData[2] = "";
						sData[3] = "";
					}
					if (!b�A���w�� && !b�i���L��) break;
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader_y);
				disposeReader(reader_h);
				reader_y = null;
				reader_h = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * ���o�׃f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n
		 *
		 *********************************************************************/
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q START
// ADD 2005.05.11 ���s�j���� ORA-03113�΍�H START
		private static string GET_UNPUBLISHED_SELECT
			= "SELECT �o�^��, \"�W���[�i���m�n\" \n"
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" \n";
		private static string GET_UNPUBLISHED_SELECT_WHERE
			= " AND ����󔭍s�ςe�f = '0' \n"
			+ " AND ���     = '01' \n"
			+ " AND �폜�e�f = '0' \n"
// ADD 2005.05.11 ���s�j���� ORA-03113�΍�H END
// ADD 2005.07.11 ���s�j���� ���x���̈�����̐ݒ� START
			+ " ORDER BY �o�ד�, �o�^��, \"�W���[�i���m�n\" \n";
// ADD 2005.07.11 ���s�j���� ���x���̈�����̐ݒ� END
//		private static string GET_UNPUBLISHED_SELECT
//			= "SELECT /*+ INDEX(ST01 ST01IDX2) INDEX(SM01 SM01PKEY) */ \n"
//			+ " ST01.�o�^��, ST01.\"�W���[�i���m�n\" \n"
//			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" ST01, �r�l�O�P�ב��l SM01 \n";
//		private static string GET_UNPUBLISHED_SELECT_WHERE
//			= " AND ST01.����󔭍s�ςe�f = '0' \n"
//			+ " AND ST01.���     = '01' \n"
//			+ " AND ST01.�폜�e�f = '0' \n"
//			+ " AND ST01.����b�c = SM01.����b�c \n"
//			+ " AND ST01.����b�c = SM01.����b�c \n"
//			+ " AND ST01.�ב��l�b�c = SM01.�ב��l�b�c \n"
//			+ " AND SM01.�폜�e�f = '0' \n"
//			+ " ORDER BY ST01.�o�ד�, ST01.�o�^��, ST01.\"�W���[�i���m�n\" \n";
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q END
		[WebMethod]
		public ArrayList Get_Unpublished(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "���o�׃f�[�^�����J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			try
			{
				//�����s�f�[�^���擾
				//StringBuilder�g�p
				System.Text.StringBuilder cmdQuery = new System.Text.StringBuilder(256);
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//				cmdQuery.Append("SELECT ");
//				cmdQuery.Append( "�o�^�� ");
//				cmdQuery.Append(",\"�W���[�i���m�n\" ");
//				cmdQuery.Append(  "FROM \"�r�s�O�P�o�׃W���[�i��\" ");
//				cmdQuery.Append( "WHERE ����b�c = '" + sKey[0] + "' ");
//				cmdQuery.Append(   "AND ����b�c = '" + sKey[1] + "' ");
//				cmdQuery.Append(   "AND ����󔭍s�ςe�f = '0' ");
//				cmdQuery.Append(   "AND ��� = '01' ");
//				cmdQuery.Append(   "AND �폜�e�f = '0' \n");
				cmdQuery.Append(GET_UNPUBLISHED_SELECT);
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q START
				cmdQuery.Append(" WHERE ����b�c = '" + sKey[0] + "' \n");
				cmdQuery.Append(  " AND ����b�c = '" + sKey[1] + "' \n");
//				cmdQuery.Append(" WHERE ST01.����b�c = '" + sKey[0] + "' \n");
//				cmdQuery.Append(  " AND ST01.����b�c = '" + sKey[1] + "' \n");
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q END
				cmdQuery.Append(GET_UNPUBLISHED_SELECT_WHERE);
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				cmdQuery = null;

				while (reader.Read())
				{
					string[] sData = new string[3];
					sData[0]  = reader.GetString(0).Trim();
					sData[1]  = reader.GetDecimal(1).ToString().Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�����͂��ׂĈ���ςł��B";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}

		/*********************************************************************
		 * �o�^�ςݑ���󖢔��s�f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�W���[�i���m�n
		 *
		 *********************************************************************/
		[WebMethod]
		public ArrayList Get_ShippedUnpublished(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "����󖢔��s�����J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}
// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			try
			{
				//�o�^�ς݁A����󖢔��s�̃f�[�^���擾
				//StringBuilder�g�p
				System.Text.StringBuilder cmdQuery = new System.Text.StringBuilder(256);
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q START
				cmdQuery.Append("SELECT ");
				cmdQuery.Append( "�o�^�� ");
				cmdQuery.Append(",\"�W���[�i���m�n\" ");
				cmdQuery.Append(  "FROM \"�r�s�O�P�o�׃W���[�i��\" ");
				cmdQuery.Append( "WHERE ����b�c = '" + sKey[0] + "' ");
				cmdQuery.Append(   "AND ����b�c = '" + sKey[1] + "' ");
				cmdQuery.Append(   "AND ����󔭍s�ςe�f = '0' ");
				cmdQuery.Append(   "AND ��� = '01' ");
				cmdQuery.Append(   "AND �폜�e�f = '0' \n");
// ADD 2005.07.11 ���s�j���� ���x���̈�����̐ݒ� START
				cmdQuery.Append(" ORDER BY �o�ד�, �o�^��, \"�W���[�i���m�n\" \n");
// ADD 2005.07.11 ���s�j���� ���x���̈�����̐ݒ� END
//				//���o�׃f�[�^�����Ɠ����r�p�k
//				cmdQuery.Append(GET_UNPUBLISHED_SELECT);
//				cmdQuery.Append(" WHERE ST01.����b�c = '" + sKey[0] + "' \n");
//				cmdQuery.Append(" AND ST01.����b�c = '" + sKey[1] + "' \n");
//				cmdQuery.Append(GET_UNPUBLISHED_SELECT_WHERE);
//�ۗ� MOD 2007.05.29 ���s�j���� ���o�׃f�[�^���̕s��v��Q END
				/*
								//string�g�p
								string cmdQuery = "SELECT "
												+  "�o�^�� "
												+  ",�W���[�i���m�n "
												+   "FROM \"�r�s�O�P�o�׃W���[�i��\" "
												+  "WHERE ����b�c = '" + sKey[0] + "' "
												+    "AND ����b�c = '" + sKey[1] + "' "
												+    "AND ����󔭍s�ςe�f = '0' "
												+    "AND ��� = '01' "
												+    "AND �폜�e�f = '0' ";
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
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�����͂��ׂĈ���ςł��B";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}

// ADD 2005.06.06 ���s�j�����J �o�׎��ш���f�[�^�擾 START
		/*********************************************************************
		 * �o�׎��ш���f�[�^�擾
		 * �����F����b�c�A����b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I����
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�׎�l�b�c...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_1
			= "SELECT J.�o�^��, J.�o�ד�, �����ԍ�, J.�׎�l�b�c, J.�X�֔ԍ�, \n"
			+       " J.�d�b�ԍ��P, J.�d�b�ԍ��Q, J.�d�b�ԍ��R, \n"
			+       " J.�Z���P, J.�Z���Q, J.�Z���R, J.���O�P, J.���O�Q, J.���X�b�c, J.���X��, \n"
			+       " J.�ב��l�b�c, NVL(N.�X�֔ԍ�, ' '), \n"
			+       " NVL(N.�d�b�ԍ��P,' '), NVL(N.�d�b�ԍ��Q,' '), NVL(N.�d�b�ԍ��R,' '), \n"
			+       " NVL(N.�Z���P,' '), NVL(N.�Z���Q,' '), NVL(N.���O�P,' '), NVL(N.���O�Q,' '), \n"
			+       " J.�ב��l������, TO_CHAR(J.��), TO_CHAR(J.�d��), \n"
			+       " J.�w���, J.�A���w���P, J.�A���w���Q, J.�i���L���P, J.�i���L���Q, J.�i���L���R, \n"
			+       " J.�����敪, TO_CHAR(J.�ی����z), J.���q�l�o�הԍ�, \n"
			+       " TO_CHAR(J.�ː�), J.�w����敪 \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			+       ", J.�^���ː�, J.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J,�r�l�O�P�ב��l N \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
			;

		private static string GET_SYUKKA_SELECT_1_SORT
			= " ORDER BY �o�^��,\"�W���[�i���m�n\" ";

		private static string GET_SYUKKA_SELECT_1_SORT2
			= " ORDER BY �o�ד�,�o�^��,\"�W���[�i���m�n\" ";

		[WebMethod]
		public ArrayList Get_PublishedPrintData(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׎��ш���f�[�^�J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
//			ArrayList sList = new ArrayList();

			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː� = 0;
			decimal d�d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.����b�c = '" + sBCode + "' \n");

				if(iSyuka == 0)
					sbQuery.Append(" AND J.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				sbQuery.Append(" AND J.�폜�e�f = '0' \n");
				sbQuery.Append(" AND J.�ב��l�b�c     = N.�ב��l�b�c(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '0' = N.�폜�e�f(+) ");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
				sbQuery.Append(" AND J.����b�c     = CM01.����b�c(+) \n");
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

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
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					string[] sData = new string[38];
					string[] sData = new string[40];
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					for(int iCnt = 0; iCnt < 38; iCnt++)
					for(int iCnt = 0; iCnt < sData.Length; iCnt++)
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
					{
// ADD 2005.09.02 ���s�j�����J Trim�͂��� START
//						sData[iCnt] = reader.GetString(iCnt).Trim();
						sData[iCnt] = reader.GetString(iCnt);
// ADD 2005.09.02 ���s�j�����J Trim�͂��� END
					}
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
					s�^���ː� = reader.GetString(38).TrimEnd();
					s�^���d�� = reader.GetString(39).TrimEnd();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(40).TrimEnd();
					if(s�d�ʓ��͐��� == "1"
					&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//					&& (sData[26].TrimEnd() != "0" || sData[36].TrimEnd() != "0")
					){
						;
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						d�ː� = 0;
						d�d�� = 0;
						if(s�^���ː�.Length > 0){
							try{
								d�ː� = Decimal.Parse(s�^���ː�);
							}catch(Exception){}
						}
						if(s�^���d��.Length > 0){
							try{
								d�d�� = Decimal.Parse(s�^���d��);
							}catch(Exception){}
						}
						sData[26] = d�d��.ToString();	// �d��
						sData[36] = d�ː�.ToString();	// �ː�
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2005.06.06 ���s�j�����J �o�׎��ш���f�[�^�擾 END

// ADD 2006.07.05 ���s�j�R�{ �A�h���X������̓͂������f�[�^�擾�Ăяo���Ή� START
		/*********************************************************************
		 * �͂������f�[�^�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�׎�l�b�c�A�J�i���́A�d�b�ԍ�...
		 *
		 *********************************************************************/
		[WebMethod]
// ADD 2007.02.14 FJCS�j�K�c ���������ɖ��O�̒ǉ� START
		public ArrayList Get_ConsigneePrintData2(string[] sUser, string[] sKey, string sKana, string sTCode, string sTelNo, string sTelNo2, string sTelNo3, string sName,int iSortLabel1,int iSortPat1,int iSortLabel2,int iSortPat2)
// MOD 2010.02.03 ���s�j���� ���������ɍX�V����ǉ� START
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
// MOD 2010.02.03 ���s�j���� ���������ɍX�V����ǉ� END
// ADD 2007.02.14 FJCS�j�K�c ���������ɖ��O�̒ǉ� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�͂������f�[�^�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();
			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//
//			// ����`�F�b�N
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
				sbQuery.Append(" �׎�l�b�c ");
				sbQuery.Append(",�J�i���� ");
				sbQuery.Append(",�d�b�ԍ��P ");
				sbQuery.Append(",�d�b�ԍ��Q ");
				sbQuery.Append(",�d�b�ԍ��R ");
				sbQuery.Append(",�X�֔ԍ� ");
				sbQuery.Append(",�Z���P ");
				sbQuery.Append(",�Z���Q ");
				sbQuery.Append(",�Z���R ");
				sbQuery.Append(",���O�P ");
				sbQuery.Append(",���O�Q ");
				sbQuery.Append(",����v \n");
				sbQuery.Append(" FROM \"�r�l�O�Q�׎�l\" \n");
				sbQuery.Append(" WHERE ����b�c = '" + sKey[0] + "' \n");
				sbQuery.Append(" AND ����b�c = '" + sKey[1] + "' \n");
				if(sKana.Length > 0 && sTCode.Length == 0)
				{
					sbQuery.Append(" AND �J�i���� LIKE '%"+ sKana + "%' \n");
				}
				if(sTCode.Length > 0 && sKana.Length == 0)
				{
					sbQuery.Append(" AND �׎�l�b�c LIKE '"+ sTCode + "%' \n");
				}
				if(sTCode.Length > 0 && sKana.Length > 0)
				{
					sbQuery.Append(" AND �J�i���� LIKE '%"+ sKana + "%' \n");
					sbQuery.Append(" AND �׎�l�b�c LIKE '"+ sTCode + "%' \n");
				}
				if(sTelNo.Length > 0)
				{
					sbQuery.Append(" AND �d�b�ԍ��P LIKE '"+ sTelNo + "%' \n");
				}
				if(sTelNo2.Length > 0)
				{
					sbQuery.Append(" AND �d�b�ԍ��Q LIKE '"+ sTelNo2 + "%' \n");
				}
				if(sTelNo3.Length > 0)
				{
					sbQuery.Append(" AND �d�b�ԍ��R LIKE '"+ sTelNo3 + "%' \n");
				}
				// ADD 2007.01.30 FJCS�j�K�c ���������ɖ��O��ǉ� START
				if(sName.Length > 0)
				{
					sbQuery.Append(" AND ���O�P LIKE '%"+ sName + "%' \n");
				}
				// ADD 2007.01.30 FJCS�j�K�c ���������ɖ��O��ǉ� END
// MOD 2010.02.03 ���s�j���� ���������ɍX�V����ǉ� START
				if(sUpdateDay.Length > 0){
					string s�X�V�����r = sUpdateDay + "000000";
					string s�X�V�����d = sUpdateDay + "999999";
					sbQuery.Append(" AND �X�V���� BETWEEN "+s�X�V�����r+" AND "+s�X�V�����d+" \n");
				}
// MOD 2010.02.03 ���s�j���� ���������ɍX�V����ǉ� END
				sbQuery.Append(" AND �폜�e�f = '0' \n");

// MOD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[�׎�l�b�c]��ǉ� START
//				if((iSortLabel1 != 0)||(iSortLabel2 != 0))
//					sbQuery.Append(" ORDER BY \n");
				sbQuery.Append(" ORDER BY \n");
// MOD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[�׎�l�b�c]��ǉ� END
				if(iSortLabel1 != 0)
				{
					switch(iSortLabel1)
					{
// UPD 2007.01.30 FJCS�j�K�c Index���ڕύX START
//						case 1:
//							sbQuery.Append(" ���O�P ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 2:
//							sbQuery.Append(" �׎�l�b�c ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 3:
//							sbQuery.Append(" �d�b�ԍ��P ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", �d�b�ԍ��Q ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", �d�b�ԍ��R ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 4:
//							sbQuery.Append(" �J�i���� ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 5:
//							sbQuery.Append(" �o�^���� ");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 6:
//							sbQuery.Append(" �X�V����");
//							if(iSortPat1 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
						case 1:
							sbQuery.Append(" �J�i���� ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 2:
							sbQuery.Append(" �׎�l�b�c ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 3:
							sbQuery.Append(" �d�b�ԍ��P ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", �d�b�ԍ��Q ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", �d�b�ԍ��R ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 4:
							sbQuery.Append(" ���O�P ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[���O�Q]��ǉ� START
							sbQuery.Append(", ���O�Q ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[���O�Q]��ǉ� END
							break;
						case 5:
							sbQuery.Append(" �o�^���� ");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 6:
							sbQuery.Append(" �X�V����");
							if(iSortPat1 == 1)
								sbQuery.Append(" DESC \n");
							break;
// UPD 2007.01.30 FJCS�j�K�c Index���ڕύX END
					}
					if(iSortLabel2 != 0)
						sbQuery.Append(" , ");
				}
				if(iSortLabel2 != 0)
				{
					switch(iSortLabel2)
					{
// UPD 2007.01.30 FJCS�j�K�c Index���ڕύX START
//						case 1:
//							sbQuery.Append(" ���O�P ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 2:
//							sbQuery.Append(" �׎�l�b�c ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 3:
//							sbQuery.Append(" �d�b�ԍ��P ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", �d�b�ԍ��Q ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							sbQuery.Append(", �d�b�ԍ��R ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 4:
//							sbQuery.Append(" �J�i���� ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 5:
//							sbQuery.Append(" �o�^���� ");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
//						case 6:
//							sbQuery.Append(" �X�V����");
//							if(iSortPat2 == 1)
//								sbQuery.Append(" DESC \n");
//							break;
						case 1:
							sbQuery.Append(" �J�i���� ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 2:
							sbQuery.Append(" �׎�l�b�c ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 3:
							sbQuery.Append(" �d�b�ԍ��P ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", �d�b�ԍ��Q ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							sbQuery.Append(", �d�b�ԍ��R ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 4:
							sbQuery.Append(" ���O�P ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[���O�Q]��ǉ� START
							sbQuery.Append(", ���O�Q ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[���O�Q]��ǉ� END
							break;
						case 5:
							sbQuery.Append(" �o�^���� ");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
						case 6:
							sbQuery.Append(" �X�V����");
							if(iSortPat2 == 1)
								sbQuery.Append(" DESC \n");
							break;
// UPD 2007.01.30 FJCS�j�K�c Index���ڕύX END
					}
				}
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[�׎�l�b�c]��ǉ� START
				if((iSortLabel1 != 0) || (iSortLabel2 != 0))
					sbQuery.Append(" , ");
				sbQuery.Append(" �׎�l�b�c \n");
// ADD 2009.01.29 ���s�j���� �ꗗ�̃\�[�g����[�׎�l�b�c]��ǉ� END

				OracleDataReader reader = CmdSelect(sUser, conn2, sbQuery);

				while (reader.Read())
				{
					string[] sData = new string[13];
					sData[0]  = reader.GetString(0).Trim();
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[1]  = reader.GetString(1).Trim();
					sData[1]  = reader.GetString(1).TrimEnd(); // �J�i����
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[2]  = reader.GetString(2).Trim();
					sData[3]  = reader.GetString(3).Trim();
					sData[4]  = reader.GetString(4).Trim();
					sData[5]  = reader.GetString(5).Substring(0,3);
					sData[6]  = reader.GetString(5).Substring(3,4);
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� START
//					sData[7]  = reader.GetString(6).Trim();
//					sData[8]  = reader.GetString(7).Trim();
//					sData[9]  = reader.GetString(8).Trim();
//					sData[10] = reader.GetString(9).Trim();
//					sData[11] = reader.GetString(10).Trim();
					sData[7]  = reader.GetString(6).TrimEnd(); //�Z���P
					sData[8]  = reader.GetString(7).TrimEnd(); //�Z���Q
					sData[9]  = reader.GetString(8).TrimEnd(); //�Z���R
					sData[10] = reader.GetString(9).TrimEnd(); //���O�P
					sData[11] = reader.GetString(10).TrimEnd();//���O�Q
// MOD 2011.01.18 ���s�j���� �Z�����O�̑OSPACE���߂Ȃ� END
					sData[12] = reader.GetString(11).Trim();
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2006.07.05 ���s�j�R�{ �A�h���X������̓͂������f�[�^�擾�Ăяo���Ή� START

// ADD 2006.08.03 ���s�j�R�{ �o�͌��ʂɉ^�����ڂ�ǉ� START
		/*********************************************************************
		 * �o�׎��ш���f�[�^�擾
		 * �����F����b�c�A����b�c�A�o�ד� or �o�^���A
		 *		 �J�n���A�I����
		 * �ߒl�F�X�e�[�^�X�A�o�^���A�׎�l�b�c...
		 *
		 *********************************************************************/
		private static string GET_SYUKKA_SELECT_2
			= "SELECT J.�o�^��, J.�o�ד�, �����ԍ�, J.�׎�l�b�c, J.�X�֔ԍ�, \n"
			+       " J.�d�b�ԍ��P, J.�d�b�ԍ��Q, J.�d�b�ԍ��R, \n"
			+       " J.�Z���P, J.�Z���Q, J.�Z���R, J.���O�P, J.���O�Q, J.���X�b�c, J.���X��, \n"
			+       " J.�ב��l�b�c, NVL(N.�X�֔ԍ�, ' '), \n"
			+       " NVL(N.�d�b�ԍ��P,' '), NVL(N.�d�b�ԍ��Q,' '), NVL(N.�d�b�ԍ��R,' '), \n"
			+       " NVL(N.�Z���P,' '), NVL(N.�Z���Q,' '), NVL(N.���O�P,' '), NVL(N.���O�Q,' '), \n"
			+       " J.�ב��l������, TO_CHAR(J.��), TO_CHAR(J.�d��), \n"
			+       " J.�w���, J.�A���w���P, J.�A���w���Q, J.�i���L���P, J.�i���L���Q, J.�i���L���R, \n"
			+       " J.�����敪, TO_CHAR(J.�ی����z), J.���q�l�o�הԍ�, \n"
//			+       " TO_CHAR(J.�ː�), J.�w����敪 \n"
			+       " TO_CHAR(J.�ː�), J.�w����敪 ,\n"
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� START
//			+       " TO_CHAR(J.�^��) \n"
			+       " TO_CHAR(J.�^�� + J.���p) \n"
// MOD 2007.10.22 ���s�j���� �^���ɒ��p�������Z�\�� END
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+       ", NVL(CM01.�L���A�g�e�f,'0') \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
			+       ", J.���Ӑ�b�c, J.���ۂb�c, J.���ۖ� \n"
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			+       ", J.�^���ː�, J.�^���d�� \n"
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			+       ", NVL(CM01.�ۗ�����e�f,'0') \n"
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
			+       ", J.�i���L���S, J.�i���L���T, J.�i���L���U \n"
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� START
			+       ", J.�����O�R \n"
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� END
			+ " FROM \"�r�s�O�P�o�׃W���[�i��\" J,�r�l�O�P�ב��l N \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
			+  ", �b�l�O�P��� CM01 \n"
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
			;

		private static string GET_SYUKKA_SELECT_2_SORT
			= " ORDER BY �o�^��,\"�W���[�i���m�n\" ";

		private static string GET_SYUKKA_SELECT_2_SORT2
			= " ORDER BY �o�ד�,�o�^��,\"�W���[�i���m�n\" ";

// ADD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� START
		private static string GET_SYUKKA_SELECT_2_SORT3
			= " ORDER BY �o�^��,�ב��l�b�c,\"�W���[�i���m�n\" ";

		private static string GET_SYUKKA_SELECT_2_SORT4
			= " ORDER BY �o�ד�,�ב��l�b�c,�o�^��,\"�W���[�i���m�n\" ";

// ADD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� END

		[WebMethod]
		public ArrayList Get_PublishedPrintData2(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday, string sIraiCd)
// ADD 2008.07.09 ���s�j���� �����s�������O���� START
		{
			return Get_PublishedPrintData3(sUser, sKCode, sBCode, iSyuka, sSday, sEday, sIraiCd, "00");
		}

		[WebMethod]
		public ArrayList Get_PublishedPrintData3(string[] sUser, string sKCode, string sBCode, 
							int iSyuka, string sSday, string sEday, string sIraiCd, string sJyoutai)
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
		{
			string[] sKey = new string[]{sKCode, sBCode, "", sIraiCd, iSyuka.ToString()
											, sSday, sEday, sJyoutai};
			return Get_PublishedPrintData4(sUser, sKey);
		}
		
		[WebMethod]
		public ArrayList Get_PublishedPrintData4(string[] sUser, string[] sKey)
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
// ADD 2008.07.09 ���s�j���� �����s�������O���� END
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "�o�׎��ш���f�[�^�S�J�n");
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
			string sKCode   = sKey[0];
			string sBCode   = sKey[1];
			string s�׎�l�b�c = sKey[2];
			string sIraiCd  = sKey[3];
			int    iSyuka   = int.Parse(sKey[4]);
			string sSday    = sKey[5];
			string sEday    = sKey[6];
			string sJyoutai = sKey[7];
			string s�����ԍ��J�n = ""; if(sKey.Length >  8) s�����ԍ��J�n = sKey[ 8];
			string s�����ԍ��I�� = ""; if(sKey.Length >  9) s�����ԍ��I�� = sKey[ 9];
			string s���q�l�ԍ��J�n = ""; if(sKey.Length > 10) s���q�l�ԍ��J�n = sKey[10];
			string s���q�l�ԍ��I�� = ""; if(sKey.Length > 11) s���q�l�ԍ��I�� = sKey[11];
			string s������b�c     = ""; if(sKey.Length > 12) s������b�c     = sKey[12];
			string s�����敔�ۂb�c = ""; if(sKey.Length > 13) s�����敔�ۂb�c = sKey[13];
			int    i�o�͌`��       = 0 ; if(sKey.Length > 14) i�o�͌`��       = int.Parse(sKey[14]);

			if(s�����ԍ��J�n.Length == 0) s�����ԍ��J�n = s�����ԍ��I��;
			if(s�����ԍ��I��.Length == 0) s�����ԍ��I�� = s�����ԍ��J�n;
			if(s���q�l�ԍ��J�n.Length == 0) s���q�l�ԍ��J�n = s���q�l�ԍ��I��;
			if(s���q�l�ԍ��I��.Length == 0) s���q�l�ԍ��I�� = s���q�l�ԍ��J�n;
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END

			OracleConnection conn2 = null;
			ArrayList alRet = new ArrayList();

			string[] sRet = new string[1];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				alRet.Add(sRet);
				return alRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				alRet.Add(sRet);
//				return alRet;
//			}

// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
			string  s�^���ː� = "";
			string  s�^���d�� = "";
			decimal d�ː� = 0;
			decimal d�d�� = 0;
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
			string  s�d�ʓ��͐��� = "0";
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END

			StringBuilder sbQuery = new StringBuilder(1024);
			StringBuilder sbQuery2 = new StringBuilder(1024);
			try
			{
				sbQuery.Append(" WHERE J.����b�c = '" + sKCode + "' \n");
				sbQuery.Append("   AND J.����b�c = '" + sBCode + "' \n");
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
				if(s�����ԍ��J�n.Length > 0){
					if(s�����ԍ��J�n == s�����ԍ��I��){
						sbQuery.Append(" AND J.�����ԍ� = '0000"+ s�����ԍ��J�n + "' \n");
					}else{
						sbQuery.Append(" AND J.�����ԍ� BETWEEN '0000"+ s�����ԍ��J�n
													 + "' AND '0000"+ s�����ԍ��I�� + "' \n");
					}
				}
				if(s���q�l�ԍ��J�n.Length > 0){
					if(s���q�l�ԍ��J�n == s���q�l�ԍ��I��){
						sbQuery.Append(" AND J.���q�l�o�הԍ� = '"+ s���q�l�ԍ��J�n + "' \n");
					}else{
						sbQuery.Append(" AND J.���q�l�o�הԍ� BETWEEN '"+ s���q�l�ԍ��J�n
													 + "' AND '"+ s���q�l�ԍ��I�� + "' \n");
					}
				}
				if(s������b�c.Length > 0){
					sbQuery.Append(" AND J.���Ӑ�b�c = '"+ s������b�c + "' \n");
				}
				if(s�����敔�ۂb�c.Length > 0){
					sbQuery.Append(" AND J.���ۂb�c = '"+ s�����敔�ۂb�c + "' \n");
				}
				if(s�׎�l�b�c.Length > 0){
					sbQuery.Append(" AND J.�׎�l�b�c = '"+ s�׎�l�b�c + "' \n");
				}
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END

// MOD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� START
//				if(iSyuka == 0)
				if(iSyuka == 0 || iSyuka == 2)
// MOD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� END
					sbQuery.Append(" AND J.�o�ד�  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				else
					sbQuery.Append(" AND J.�o�^��  BETWEEN '"+ sSday + "' AND '"+ sEday +"' \n");
				
				if(sIraiCd.Trim().Length != 0)
				{
					sbQuery.Append(" AND '" + sIraiCd + "' = J.�ב��l�b�c(+) \n");
				}
				
				sbQuery.Append(" AND J.�폜�e�f = '0' \n");
				sbQuery.Append(" AND J.�ב��l�b�c     = N.�ב��l�b�c(+) \n");
				sbQuery.Append(" AND '" + sKCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '" + sBCode + "' = N.����b�c(+) \n");
				sbQuery.Append(" AND '0' = N.�폜�e�f(+) ");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
				sbQuery.Append(" AND J.����b�c = CM01.����b�c(+) \n");
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
// ADD 2008.07.09 ���s�j���� �����s�������O���� START
				if(sJyoutai != "00"){
					if(sJyoutai == "aa"){
						sbQuery.Append(" AND J.��� <> '01' \n");
					}else{
						sbQuery.Append(" AND J.��� = '"+ sJyoutai + "' \n");
					}
				}
// ADD 2008.07.09 ���s�j���� �����s�������O���� END

				OracleDataReader reader;
// MOD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� START
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
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
				switch(i�o�͌`��){
				case 1:		//���˗����
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT4);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT3);
					}
					break;
				case 2:		//�������
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY �o�ד�,���Ӑ�b�c,���ۂb�c,�o�^��,\"�W���[�i���m�n\" ");
					}else{
						sbQuery2.Append(" ORDER BY �o�^��,���Ӑ�b�c,���ۂb�c,\"�W���[�i���m�n\" ");
					}
					break;
				case 3:		//���͂����
					if(iSyuka == 0){
						sbQuery2.Append(" ORDER BY �o�ד�,�׎�l�b�c,�o�^��,\"�W���[�i���m�n\" ");
					}else{
						sbQuery2.Append(" ORDER BY �o�^��,�׎�l�b�c,\"�W���[�i���m�n\" ");
					}
					break;
				default:	//�w��Ȃ�
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
					if(iSyuka == 0){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT2);
					}else if(iSyuka == 2){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT4);
					}else if(iSyuka == 3){
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT3);
					}else{
						sbQuery2.Append(GET_SYUKKA_SELECT_2_SORT);
					}
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
					break;
				}
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
				reader = CmdSelect(sUser, conn2, sbQuery2);
// MOD 2009.02.02 ���s�j���� ���шꗗ�̃\�[�g����[�ב��l�b�c]��ǉ� END

				while (reader.Read())
				{
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
//					string[] sData = new string[39];
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
//					string[] sData = new string[42];
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
//					string[] sData = new string[44];
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� START
//					string[] sData = new string[47];
					string[] sData = new string[48];
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
					for(int iCnt = 0; iCnt < 39; iCnt++)
					{
						sData[iCnt] = reader.GetString(iCnt);
					}
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� START
					if(reader.GetString(39).Equals("1")){
						sData[38] = "0";
					}
// MOD 2009.05.28 ���s�j���� �o�׎��шꗗ�^����\���Ή� END
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� START
					sData[39] = reader.GetString(40);	//���Ӑ�b�c
					sData[40] = reader.GetString(41);	//���ۂb�c
					sData[41] = reader.GetString(42);	//���ۖ�
// MOD 2009.11.06 ���s�j���� ���������ɐ�����A���͂���A���q�l�ԍ���ǉ� END
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� START
					sData[42] = reader.GetString(43);
					sData[43] = reader.GetString(44);
					s�^���ː� = reader.GetString(43).TrimEnd();
					s�^���d�� = reader.GetString(44).TrimEnd();
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					s�d�ʓ��͐��� = reader.GetString(45).TrimEnd();
					if(s�d�ʓ��͐��� == "1"
					&& s�^���ː�.Length == 0 && s�^���d��.Length == 0
//					&& (sData[26].TrimEnd() != "0" || sData[36].TrimEnd() != "0")
					){
						sData[42] = sData[36]; // �ː�
						sData[43] = sData[26]; // �d��
					}else{
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
//�ۗ�						if(s�d�ʓ��͐��� == "1"
//�ۗ�						&& sData[42] == "     " && sData[43] == "     "
//�ۗ�						){
//�ۗ�							sData[42] = sData[36]; // �ː�
//�ۗ�							sData[43] = sData[26]; // �d��
//�ۗ�						}
//�ۗ� MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
						d�ː� = 0;
						d�d�� = 0;
						if(s�^���ː�.Length > 0){
							try{
								d�ː� = Decimal.Parse(s�^���ː�);
							}catch(Exception){}
						}
						if(s�^���d��.Length > 0){
							try{
								d�d�� = Decimal.Parse(s�^���d��);
							}catch(Exception){}
						}
						sData[26] = d�d��.ToString();	// �d��
						sData[36] = d�ː�.ToString();	// �ː�
// MOD 2011.04.13 ���s�j���� �d�ʓ��͕s�Ή� END
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� START
					}
// MOD 2011.05.06 ���s�j���� ���q�l���Ƃɏd�ʓ��͐��� END
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� START
					sData[44] = reader.GetString(46).TrimEnd(); // �i���L���S
					sData[45] = reader.GetString(47).TrimEnd(); // �i���L���T
					sData[46] = reader.GetString(48).TrimEnd(); // �i���L���U
// MOD 2011.07.14 ���s�j���� �L���s�̒ǉ� END
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� START
					sData[47] = reader.GetString(49).Trim();	// �z�����t�E����
// MOD 2013.10.07 BEVAS�j���� �o�׎��шꗗ�\�ɔz�����t�E������ǉ� END
					alRet.Add(sData);
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				if (alRet.Count == 0)
				{
					sRet[0] = "�Y���f�[�^������܂���";
					alRet.Add(sRet);
				}
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				alRet.Insert(0, sRet);
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return alRet;
		}
// ADD 2006.08.03 ���s�j�R�{ �o�͌��ʂɉ^�����ڂ�ǉ� END


// ADD 2014.09.10 BEVAS)�O�c �x�X�~�ߋ@�\�ǉ��Ή� START
		/*********************************************************************
		 * �x�X�~�ߑΉ��X���ꗗ�擾
		 * �����F�X���b�c�^�X����, ����/���q�t���O 
		 * ("1" = ���R�ʉ^����̎x�X�~�ߑΉ� / "2" = ���q�^�A����̎x�X�~�ߑΉ�)
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X�����A�Z��
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_DeliShop(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "�x�X�~�ߑΉ��X���ꗗ�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT '|'"
					+     " || TRIM(�X���b�c) || '|' "
					+     " || TRIM(�X����) || '|' "
					+     " || TRIM(�X��������) || '|' "
					+     " || TRIM(�X�֔ԍ�) || '|' \n"
					+     "\n"
					+  " FROM �b�l�P�O�X�� \n";
				
				if (sKey[2].Equals("2")) 
				{
					// ���q�^�A����̎x�X�~�߂ɑΉ�����X��������
					cmdQuery += " WHERE �x�X�~�߂e�f�Q = '1' \n";
				} 
				else 
				{
					// ���R�ʉ^����̎x�X�~�߂ɑΉ�����X��������
					cmdQuery += " WHERE �x�X�~�߂e�f�P = '1' \n";
				}

				if (sKey[0].Length == 4)
				{
					cmdQuery += " AND �X���b�c = '" + sKey[0] + "' \n";
				}
				else if (sKey[0].Length > 0) 
				{
					cmdQuery += " AND �X���b�c LIKE '" + sKey[0] + "%' \n";				
				} 
				else 
				{
					// �Ȃɂ����Ȃ�
				}
				if (sKey[1].Length > 0)
				{
					cmdQuery += " AND �X���� LIKE '" + sKey[1] + "%' \n";
				} 
				else 
				{
					// �Ȃɂ����Ȃ�
				}
				cmdQuery += " AND �폜�e�f = '0' \n"
					     +  " AND NVL(LENGTH(TRIM(�X�֔ԍ�)),0) = 7 \n"
						 +  " ORDER BY �X���b�c \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �X���}�X�^����X�����y�їX�֔ԍ����擾
		 * �����F�X���b�c
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�C�X����,�X�֔ԍ�
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ShopZip(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "�X���X�֔ԍ��擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT "
					+     "   TRIM(�X���b�c) "
					+     ",  TRIM(�X����) "
					+     ",  �X�֔ԍ� "
					+     "\n"
					+  " FROM �b�l�P�O�X�� \n"
					+  " WHERE �X���b�c = '" + sKey[0] + "' \n"
					+  " AND �폜�e�f = '0' \n";

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
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �X�����������擾
		 * �����F�X���b�c
		 * �ߒl�F�X�e�[�^�X�A�X��������
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_ShopType(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "�x�X��ʎ擾�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];

			if (sKey[0].Length != 3) 
			{
				sRet[0] = "�X���b�c�̌`�����s���ł��B";
				return sRet;
			}

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				sRet[1] = "";
				return sRet;
			}

			string cmdQuery = "";
			try
			{				
				cmdQuery
					= "SELECT \n"
					+     " TRIM(�X��������) \n"
					+  " FROM �b�l�P�O�X�� \n"
					+  " WHERE �폜�e�f = '0' \n"
					+  " AND LENGTH(TRIM(�X�֔ԍ�)) = 7 \n"
					+  " AND �X���b�c = '" + sKey[0] + "' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				reader.Read();

				// �X����������Ԃ�
				sRet[1] = reader.GetString(0);

				disposeReader(reader);
				reader = null;
				sRet[0] = "����I��";
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
				sRet[1] = "";
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
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
		 * �s���{�����̎x�X�~�ߑΉ��X���ꗗ�擾
		 * �����F�s���{����, ����/���q�t���O 
		 * ("1" = ���R�ʉ^����̎x�X�~�ߑΉ� / "2" = ���q�^�A����̎x�X�~�ߑΉ�)
		 * �ߒl�F�X�e�[�^�X�A�X���b�c�A�X����
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_PrefDeliShop(string[] sUser, string[] sKey)
		{
			logWriter(sUser, INF, "�x�X�~�ߑΉ��X���ꗗ�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[4];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
			
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT '|' "
					+     " || TRIM(�X���b�c) || '|' "
					+     " || TRIM(�X����) || '|' "
					+     " || TRIM(�X��������) || '|' "
					+     " || TRIM(�X�֔ԍ�) || '|' \n"
					+  " FROM �b�l�P�O�X�� \n"
					+ " WHERE �Z�� LIKE '" + sKey[0] + "%' \n";
				if (sKey[1].Equals("2")) 
				{
					// ���q�^�A����̎x�X�~�߂ɑΉ�����X��������
					cmdQuery += " AND �x�X�~�߂e�f�Q = '1' \n";
				} 
				else 
				{
					// ���R�ʉ^����̎x�X�~�߂ɑΉ�����X��������
					cmdQuery += " AND �x�X�~�߂e�f�P = '1' \n";
				}
				cmdQuery += " AND �폜�e�f = '0' \n"
					     +  " AND LENGTH(TRIM(�X�֔ԍ�)) = 7 \n"
						 +  " ORDER BY �X���b�c \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
					sList.Add(reader.GetString(0));
				}

				disposeReader(reader);
				reader = null;

				sRet = new string[sList.Count + 1];
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
				conn2 = null;
			}
			return sRet;
		}
// ADD 2014.09.10 BEVAS)�O�c �x�X�~�ߋ@�\�ǉ��Ή� END

	}
}
