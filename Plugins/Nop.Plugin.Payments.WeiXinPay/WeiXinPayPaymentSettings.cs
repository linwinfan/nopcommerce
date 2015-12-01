using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.WeiXinPay
{
    public class WeiXinPayPaymentSettings : ISettings
    {
        /// <summary>
        /// �������û�Ψһƾ֤ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// �������û�Ψһƾ֤��Կ����appsecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// �̻�ID
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// �̻�֧����ԿKey����¼΢���̻���̨��������Ŀ���˻����á������밲ȫ����API ��ȫ����API ��Կ��
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// ֧����ɺ�Ļص�����ҳ��
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// ֤��·���������˿������ʱ��Ҫ
        /// </summary>
        public string SSLCertPath { get; set; }

        /// <summary>
        /// ֤�����룬�����˿������ʱ��Ҫ
        /// </summary>
        public string SSLCertPassword { get; set; }

        /// <summary>
        /// �̻�ϵͳ��̨����IP
        /// </summary>
        public string IP { get; set; }

        //=======��������������á�===================================
        /* Ĭ��IP�Ͷ˿ںŷֱ�Ϊ0.0.0.0��0����ʱ����������������Ҫ�����ã�
        */
        public static string PROXY_URL = "http://10.152.18.220:8080";

        //=======���ϱ���Ϣ���á�===================================
        /* �����ϱ��ȼ���0.�ر��ϱ�; 1.������ʱ�ϱ�; 2.ȫ���ϱ�
        */
        public static int REPORT_LEVENL = 0;

    }
    
}
