using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace HentaiCore {
    public class Json {
        public JObject jsonObject = null;

        public Json() {
            this.jsonObject = new JObject();

        }

        public Json(string jsonStr) {
            this.Input(jsonStr);

        }

        #region Private
        private void EditData(bool newData, string name, string value) {
            bool hasData = this.jsonObject.ContainsKey(name);

            if (hasData == newData) {
                if (hasData) {
                    throw new Exception("已經包含此 KEY");

                } else {
                    throw new Exception("找不到此 KEY");

                }

            } else if (hasData) {
                this.jsonObject[name] = value;

            } else if (newData) {
                this.jsonObject.Add(name, value);

            }

        }

        private void EditData(bool newData, string name, int value) {
            bool hasData = this.jsonObject.ContainsKey(name);

            if (hasData == newData) {
                if (hasData) {
                    throw new Exception("已經包含此 KEY");

                } else {
                    throw new Exception("找不到此 KEY");

                }

            } else if (hasData) {
                this.jsonObject[name] = value;

            } else if (newData) {
                this.jsonObject.Add(name, value);

            }

        }

        private void EditData(bool newData, string name, decimal value) {
            bool hasData = this.jsonObject.ContainsKey(name);

            if (hasData == newData) {
                if (hasData) {
                    throw new Exception("已經包含此 KEY");

                } else {
                    throw new Exception("找不到此 KEY");

                }

            } else if (hasData) {
                this.jsonObject[name] = value;

            } else if (newData) {
                this.jsonObject.Add(name, value);

            }

        }

        private void EditData(bool newData, string name, JToken value) {
            bool hasData = this.jsonObject.ContainsKey(name);

            if (hasData == newData) {
                if (hasData) {
                    throw new Exception("已經包含此 KEY");

                } else {
                    throw new Exception("找不到此 KEY");

                }

            } else if (hasData) {
                this.jsonObject[name] = value;

            } else if (newData) {
                this.jsonObject.Add(name, value);

            }

        }

        private void Input(string jsonStr) {
            this.jsonObject = JsonConvert.DeserializeObject<JObject>(jsonStr);

        }

        private string Output() {
            return this.jsonObject.ToString();

        }

        #endregion Private

        #region Public
        public bool Contain(string key) {
            return this.jsonObject.ContainsKey(key);

        }

        public void Edit(string name, string value) {
            this.EditData(false, name, value);

        }

        public void Edit(string name, int value) {
            this.EditData(false, name, value);

        }

        public void Edit(string name, decimal value) {
            this.EditData(false, name, value);

        }

        public void Edit(string name, JToken value) {
            this.EditData(false, name, value);

        }

        public void Edit(string name, JObject value)
        {
            this.EditData(false, name, value);

        }
        public void Add(string name, string value) {
            this.EditData(true, name, value);

        }

        public void Add(string name, int value) {
            this.EditData(true, name, value);

        }

        public void Add(string name, decimal value) {
            this.EditData(true, name, value);

        }

        public void Add(string name, JToken value) {
            this.EditData(true, name, value);

        }

        public void Add(string name, JObject value)
        {
            this.EditData(true, name, value);

        }

        public override string ToString() {
            return this.Output();

        }

        #endregion Public
        
    }
}
