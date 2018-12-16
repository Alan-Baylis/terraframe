using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

    class CustomTextWriter : TextWriter
    {
        public override void WriteLine(string value)
        {
            base.WriteLine(value);


            UnityEngine.Debug.Log(value);
        }

        public override Encoding Encoding
        {
            get
            {

                return Encoding.UTF8;
            }
        }
    }

