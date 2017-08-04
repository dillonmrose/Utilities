using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Tools
{

    [DataContract]
    public class Data
    {
        public Data()
        {
            value = new Dictionary<string, string>();
        }
        public Data(string header, string s)
        {
            value = new Dictionary<string, string>();
            for(int i = 0; i < header.Split('\t').Length; i++)
            {
                value[header.Split('\t')[i]] = s.Split('\t')[i];
            }
        }
        /// <summary>
        /// Gets or sets the entityId
        /// </summary>
        [DataMember]
        public Dictionary<string, string> value { get; set; }

    }

    [DataContract]
    public class Dataset
    {
        public Dataset()
        {
            data = new List<Data>();
        }
        public Dataset(string filename)
        {
            data = new List<Data>();
            StreamReader reader = new StreamReader(filename);
            header = reader.ReadLine();
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                data.Add(new Data(header, line));
            }
            reader.Close();
        }

        [DataMember]
        public List<Data> data { get; set; }

        [DataMember]
        public string header { get; set; }

        //Static methods
        public static Dataset Join(Dataset dataset1, Dataset dataset2, string column1, string column2)
        {
            Dataset output = new Dataset();
            foreach (Data d1 in dataset1.data)
            {
                foreach (Data d2 in dataset2.data)
                {
                    if (d1.value[column1] == d2.value[column2])
                    {
                        Data temp = new Data();
                        foreach (string key in d1.value.Keys)
                        {
                            temp.value[key] = d1.value[key];
                        }
                        foreach (string key in d2.value.Keys)
                        {
                            if (d1.value.Keys.Contains(key))
                            {
                                temp.value[key + "2"] = d2.value[key];
                            }
                            else
                            {
                                temp.value[key] = d2.value[key];
                            }
                        }
                        output.data.Add(temp);
                    }
                }
            }
            return output;
        }

        public static void Filter(Dataset dataset, string column, string value)
        {
            for (int i = 0; i < dataset.data.Count; i++)
            {
                if (dataset.data[i].value[column] != value)
                {
                    dataset.data.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void Filter(Dataset dataset, string column, List<string> values)
        {
            for (int i = 0; i < dataset.data.Count; i++)
            {
                bool remove = true;
                foreach (string value in values)
                {
                    if (dataset.data[i].value[column] == value)
                    {
                        remove = false;
                    }
                }
                if (remove)
                {
                    dataset.data.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void Save(Dataset dataset, string filename, List<string> header)
        {
            StreamWriter writer = new StreamWriter(filename);
            writer.WriteLine(string.Join("\t", header.ToArray()));
            foreach (Data d in dataset.data)
            {
                List<string> temp = new List<string>();
                foreach (string h in header)
                {
                    if (d.value.ContainsKey(h))
                    {
                        temp.Add(d.value[h]);
                    }
                    else
                    {
                        temp.Add("");
                    }
                }
                writer.WriteLine(string.Join("\t", temp.ToArray()));
            }
            writer.Close();
        }

        public static void Save(Dataset dataset, string filename)
        {
            StreamWriter writer = new StreamWriter(filename);

            writer.WriteLine(string.Join("\t", dataset.data[0].value.Keys.ToArray()));
            foreach (Data d in dataset.data)
            {
                List<string> temp = new List<string>();
                foreach (string key in d.value.Keys)
                {
                    if (d.value.ContainsKey(key))
                    {
                        temp.Add(d.value[key]);
                    }
                    else
                    {
                        temp.Add("");
                    }
                }
                writer.WriteLine(string.Join("\t", temp.ToArray()));
            }
            writer.Close();
        }
    }
}
