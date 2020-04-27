using GalaSoft.MvvmLight;

namespace SQLiteWPF.Model
{
    public class CompanyModel : ObservableObject
    {
        private int number;
        /// <summary>
        /// 公司名
        /// </summary>
        public int Number
        {
            get { return number; }
            set { number = value; RaisePropertyChanged(() => Number); }
        }

        private string name;
        /// <summary>
        /// 公司名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(() => Name); }
        }

        private string address;
        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; RaisePropertyChanged(() => Address); }
        }

        private string telephone;
        /// <summary>
        /// 公司电话
        /// </summary>
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; RaisePropertyChanged(() => Telephone); }
        }

        private string legalPerson;

        /// <summary>
        /// 公司法人
        /// </summary>
        public string LegalPerson
        {
            get { return legalPerson; }
            set { legalPerson = value; RaisePropertyChanged(() => LegalPerson); }
        }

        private string registrationDate;

        /// <summary>
        /// 公司注册时间
        /// </summary>
        public string RegistrationDate
        {
            get { return registrationDate; }
            set { registrationDate = value; RaisePropertyChanged(() => RegistrationDate); }
        }
    }
}
