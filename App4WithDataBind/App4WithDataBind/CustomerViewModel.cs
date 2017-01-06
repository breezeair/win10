using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace App4WithDataBind
{
    public class CustomerViewModel
    {
        private Customer defaultRecording = new Customer("a","b","c");
        public Customer DefaultRecording { get { return this.defaultRecording; } }

        private ObservableCollection<Customer> obRecordings = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> ObRecordings { get { return this.obRecordings; } }

        private Collection<Customer> oriRecordings = new Collection<Customer>();
        public Collection<Customer> OriRecordings { get { return this.oriRecordings; } }

        private ObservableCollection<Customer> obRecordings2;
        public ObservableCollection<Customer> ObRecordings2 { get { return this.obRecordings2; } }
        private Collection<Customer> oriRecordings2 = new Collection<Customer>();
        public Collection<Customer> OriRecordings2 { get { return this.oriRecordings2; } }

        public CustomerViewModel()
        {
            this.ObRecordings.Add(new Customer("李", "小明",

                    "北京市"));

            this.ObRecordings.Add(new Customer("林", "关关",

                    "深圳市"));

            this.OriRecordings.Add(new Customer("张", "三三",

                    "广州市"));

            this.OriRecordings.Add(new Customer("李", "思思",

                    "上海市"));

            obRecordings2 = new ObservableCollection<Customer>(OriRecordings);
            oriRecordings2 = new Collection<Customer>(ObRecordings);
        }
    }
}
