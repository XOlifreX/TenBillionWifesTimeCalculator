using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TenBillionWifesTimeWatcher
{
    public partial class Form1 : Form
    {

        private const double MAX_VAL = 18446744073709551615;
        private const double WoWconstRaise = 30000000000000;
        private const double oneItteration = 1000000000000000;
        private double biggerThenVal = 0;
        private double lps = 0;
        private double timeNextLevel = 0;
        private double[] timeNextRoundVal = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private double[] timeNextRoundValResult = new double[10];

        private double timeMaxVal = 0;
        private double runningLPS = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public bool checkEmptyText()
        {

            if (LPStxt.Text.Length > 0)
                return true;
            else
                return false;

        }

        public void clearEverything()
        {

            this.biggerThenVal = 0;
            this.lps = 0;
            this.timeNextLevel = 0;
            this.timeNextRoundVal = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            this.timeNextRoundValResult = new double[10];
            this.timeMaxVal = 0;
            this.runningLPS = 0;

        }

        public void setLPS(double lps)
        {
            this.lps = lps;
        }

        public void calcAllVals()
        {
            
            //time to next level;
            this.timeNextLevel = (MAX_VAL / this.lps) / 60;

            //time next round value;
            bool found = false;
            double toCheck = 0;

            for(int i = 0; !found; i++) //Look where your LPS is to itterate in the roundVal array later.
            {
                if (this.lps < toCheck)
                    found = true;
                else
                    toCheck += oneItteration; ;
            }

            this.biggerThenVal = toCheck - oneItteration;
            
            //Calculate 10 steps ahead of where you are
            this.runningLPS = this.lps;
            for (int i = 1; i < 11; i++) //for statement to count the 10 steps
            {
                for (double j = this.runningLPS; j < (biggerThenVal + (oneItteration*i)); j += WoWconstRaise) //Checks how long it takes to complete one step
                {
                    this.timeNextRoundVal[i-1] += ((((MAX_VAL / j) / 60) / 60) / 24);
                    this.runningLPS = j;
                }

                if (i-1 > 0 )
                {
                    for (int j = 0; j <= i - 1; j++) //For statement to calculate how long it takes to get to a chosen step from where you currently are
                        this.timeNextRoundValResult[i - 1] += this.timeNextRoundVal[j];
                }
                else
                    this.timeNextRoundValResult[i-1] = this.timeNextRoundVal[i-1];
            }

            //Calculate time to Max Value;
            for(double i = this.lps; i < MAX_VAL; i += WoWconstRaise)
                this.timeMaxVal += ((((MAX_VAL / i) / 60) / 60) / 24);
        }

        public void infoToListBox()
        {
            //Print function. Prints all the information in a listbox.
            resultBox.Items.Clear();

            resultBox.Items.Add(String.Format("Time To Next Level: {0:0.00} hours. ({1:00.00} minutes)", this.timeNextLevel/60, this.timeNextLevel));

            for (int i = 0; i < 10; i++)
            {
                if (i < this.timeNextRoundVal.Length)
                    resultBox.Items.Add(String.Format("Time To {0:00.0}: {1:00.00} days, {2:0.00} hours ({3:0.00} days, {4:0.00} hours).",
                        (this.biggerThenVal + (oneItteration * (i+1))) / 1000000000000000, this.timeNextRoundValResult[i], this.timeNextRoundValResult[i] * 24, this.timeNextRoundVal[i], this.timeNextRoundVal[i] * 24));
            }

            resultBox.Items.Add(String.Format("Time To MAX_VALUE: {0:000.00} days.", this.timeMaxVal));

        }

        private void calcBtn_Click(object sender, EventArgs e)
        {
            if (this.checkEmptyText())
            {

                this.clearEverything();
                this.setLPS(Double.Parse(LPStxt.Text));

                this.calcAllVals();
                this.infoToListBox();

            }
        }

    }
}
