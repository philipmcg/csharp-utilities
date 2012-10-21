using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;

using Utilities.GCSV;

namespace Utilities
{

    using Options = IEnumerable<KeyValuePair<string, string>>;

    public class OptionList
    {
        Utilities.VariableBin Var;
        List<Option> Options;

        class Option
        {
            public OptionList Parent;
            public string ID;
            public string Caption;
            public ComboBox ComboBox;
            public Label Label;
            public List<KeyValuePair<string, string>> Values;
            public bool Active;
            public string Default;

            public Option()
            {
                ComboBox = new ComboBox();
                Label = new Label();
            }

            public void Set()
            {
                Parent.Var.Str[ID] = Values[ComboBox.SelectedIndex].Key;
            }
            public void SetDefault()
            {
                Parent.Var.Str[ID] = Default;
                ComboBox.SelectedIndex = Values.FindIndex(p => p.Key == Default);
            }
            public void Initialize()
            {
                var cb = ComboBox;
                cb.Items.Clear();
                foreach (var item in Values)
                    cb.Items.Add(item.Value);
                cb.SelectedIndex = 0;

                cb.SelectionChangeCommitted += new EventHandler(cb_SelectionChangeCommitted);

                Label.Text = Caption;
            }

            void cb_SelectionChangeCommitted(object sender, EventArgs e)
            {
                if(ComboBox.SelectedIndex >= 0)
                    Set();
            }
        }

        public OptionList(GCSVTable csv, IGCSVCollection csvs, Utilities.VariableBin var, Func<IData,bool> selector)
        {
            Var = var;
            Options = new List<Option>();

            foreach (var line in csv)
            {
                if (!selector(line))
                    continue;

                var o = new Option();
                o.Parent = this;
                o.Caption = line["caption"];
                o.ID = line["id"];
                o.Active = line["active"].ToBool();
               
                o.Default = line["default"];
                var l = csvs[line["list"]];
                o.Values = l.Where(m => !m.ContainsKey("active") || m["active"].ToBool()).Select(m => new KeyValuePair<string, string>(m[line["value"]], m[line["text"]])).ToList();
                o.Initialize();

                if (!var.Str.ContainsKey(o.ID) || !o.Active) // set defaults if variable is not set or option is not active
                    var.Str[o.ID] = o.Default;

                if (var.Str.ContainsKey(o.ID))
                    o.ComboBox.SelectedIndex = o.Values.FindIndex(p => p.Key == var.Str[o.ID]);

                Options.Add(o);
            }
        }

        public OptionList(GCSVTable csv, Func<IData, Options, string> getDefault, Func<string, Options> getList, Utilities.VariableBin var, Func<IData, bool> selector)
        {
            Var = var;
            Options = new List<Option>();

            foreach (var line in csv)
            {
                if (!selector(line))
                    continue;

                var o = new Option();
                o.Parent = this;
                o.Caption = line["caption"];
                o.ID = line["id"];
                o.Active = line["active"].ToBool();
                var l = getList(line["list"]);
                o.Values = l.ToList();
                o.Default = getDefault(line, o.Values);
                o.Initialize();

                if (!var.Str.ContainsKey(o.ID))
                    var.Str[o.ID] = o.Default;

                if (var.Str.ContainsKey(o.ID))
                    o.ComboBox.SelectedIndex = o.Values.FindIndex(p => p.Key == var.Str[o.ID]);

                Options.Add(o);
            }
        }


        public void SetOptions()
        {
            foreach (var o in Options)
            {
                if (o.Active)
                    o.Set();
                else
                    o.SetDefault();
            }
        }
        
        public void SetDefaultVariablesIfEmpty()
        {
            foreach (var o in Options)
            {
                if (!Var.Str.ContainsKey(o.ID))
                    Var.Str[o.ID] = o.Default;
            }
        }
        public void SetDefaults()
        {
            foreach (var o in Options)
            {
                o.SetDefault();
            }
        }

        public void SetToPanel(Panel panel)
        {
            int n = 0;
            foreach (var o in Options)
            {
                var lbl = o.Label;
                lbl.AutoSize = true;
                lbl.Location = new System.Drawing.Point(3, 6);
                lbl.Size = new System.Drawing.Size(35, 13);
                lbl.TabIndex = 1;

                var cb = o.ComboBox;
                cb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                cb.FormattingEnabled = true;
                cb.Location = new System.Drawing.Point(180, 3);
                cb.Size = new System.Drawing.Size(panel.Width - cb.Left - 10, 21);
                cb.TabIndex = 0;

                int offset = (cb.Height + 2);
                cb.Top += n * offset;
                lbl.Top += n * offset;
                panel.Parent.Height += offset;
                //panel.Height += offset;

                panel.Controls.Add(cb);
                panel.Controls.Add(lbl);

                n++;
            }
        }

    }
}
