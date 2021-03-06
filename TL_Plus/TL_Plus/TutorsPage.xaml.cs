﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TL_Plus
{
    public partial class TutorsPage : Window
    {
        static List<TutorShift> activelyGiving;
        static List<TutorShift> activelyTaking;
        static List<TutorShift> completedRequest;


        public TutorsPage()
        {
            InitializeComponent();
            activelyGiving = new List<TutorShift>();
            activelyTaking = new List<TutorShift>();
            completedRequest = new List<TutorShift>();

        }

        private void Action_Save(object sender, RoutedEventArgs e)
        {


            // if(upload.wasSuccessful()){   <- pseudocode to verify it worked correctly
            MessageBox.Show("Your entry has been saved into the system");
        }
        private void Action_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TRequestButton(object sender, MouseButtonEventArgs e)
        {
            TutorRequest newsesh = new TutorRequest();
            newsesh.Owner = this;
            newsesh.ShowDialog();

            if (newsesh.newShift != null)
            {

                if (newsesh.newShift.givingshift == true)
                {// giving a shift
                    activelyGiving.Add(newsesh.newShift);
                    givingshift.Items.Add(newsesh.newShift.initiatorName + " from " + newsesh.newShift.start.ToString() + " to " + newsesh.newShift.end.ToString() + " on " + newsesh.newShift.date);
                   //Open mySQL connection for sending data
                    MySQLConnect connect = new MySQLConnect();
                    connect.TutorInsert(newsesh.newShift.initiatorName, true, newsesh.newShift.date, newsesh.newShift.start.ToString(), newsesh.newShift.end.ToString());
                    //Test for populating shifts, mostly working
                    /*
                    List<TutorShift> tempGiving = new List<TutorShift>();
                    tempGiving = connect.ShifsPopulate("04/28/2016", true, false);
                    for(int i =0; i< tempGiving.Count; ++i){
                        givingshift.Items.Add(tempGiving[i].initiatorName + " from " + tempGiving[i].start + " to " + tempGiving[i].end + " on " + tempGiving[i].date);
                    }*/
                }
                else if (newsesh.newShift.givingshift == false)
                {// taking a shift
                    activelyTaking.Add(newsesh.newShift);
                    takingshift.Items.Add(newsesh.newShift.initiatorName + " from " + newsesh.newShift.start.ToString() + " to " + newsesh.newShift.end.ToString() + " on " + newsesh.newShift.date);
                     MySQLConnect connect = new MySQLConnect();
                     connect.TutorInsert(newsesh.newShift.initiatorName, false, newsesh.newShift.date, newsesh.newShift.start.ToString(), newsesh.newShift.end.ToString());
                }

            }
        }

        
        private void givingshift_SelectionChanged(object sender, SelectionChangedEventArgs e)//un needed
        {
            takingshift.UnselectAll();
            tradedshifts.UnselectAll();
        }

        private void takingshift_SelectionChanged(object sender, SelectionChangedEventArgs e)//un needed
        {
            givingshift.UnselectAll();
            tradedshifts.UnselectAll();
        }

        private void tradedshifts_SelectionChanged(object sender, SelectionChangedEventArgs e)//un needed
        {
            int index = tradedshifts.SelectedIndex;
            givingshift.UnselectAll();
            takingshift.UnselectAll();
        }
        

        //static List<TutorShift> activelyGiving;
        //static List<TutorShift> activelyTaking;
        //static List<TutorShift> completedRequest;/
        private void TAcceptButton(object sender, MouseButtonEventArgs e)
        {
            int gindex = givingshift.SelectedIndex;
            int takeindex = takingshift.SelectedIndex;
            int tradeindex = tradedshifts.SelectedIndex;
            TutorAccept comp;

            if (gindex != -1 && takeindex == -1 && tradeindex == -1)
            {//section for giving column
                comp = new TutorAccept(activelyGiving[gindex]);
                comp.Owner = this;
                comp.ShowDialog();
                if (comp.submitted == true)
                {
                    activelyGiving.RemoveAt(gindex);
                    completedRequest.Add(comp.shift);// remove old version, add new

                    givingshift.Items.RemoveAt(gindex);// remove old list item, replace with updated text
                    tradedshifts.Items.Add(comp.shift.initiatorName + " " + comp.shift.start + " - " + comp.shift.end + ", " + comp.shift.date + " (Taken by " + comp.shift.acceptorName + ")");
                    MySQLConnect connect = new MySQLConnect();
                    connect.TutorUpdate(comp.shift.initiatorName, comp.shift.date, comp.shift.start, comp.shift.end, true, comp.shift.acceptorName);
                }


            }
            else if (gindex == -1 && takeindex != -1 && tradeindex == -1)
            {// section for taking column
                comp = new TutorAccept(activelyTaking[takeindex]);
                comp.Owner = this;
                comp.ShowDialog();

                if (comp.submitted == true)
                {
                    activelyTaking.RemoveAt(takeindex);
                    completedRequest.Add(comp.shift);// remove old version, add new

                    takingshift.Items.RemoveAt(takeindex);// remove old list item, replace with updated text
                    tradedshifts.Items.Add(comp.shift.initiatorName + " " + comp.shift.start + "-" + comp.shift.end + comp.shift.date + " (Taken by " + comp.shift.acceptorName + ")");
                    MySQLConnect connect = new MySQLConnect();
                    connect.TutorUpdate(comp.shift.initiatorName, comp.shift.date, comp.shift.start, comp.shift.end, true, comp.shift.acceptorName);
                }
            }
            else if (gindex == -1 && takeindex == -1 && tradeindex != -1)
            {// section for already traded column
                // TutorAccept comp = new TutorAccept(completedRequest[tradeindex]);
                // comp.Owner = this;
                // comp.ShowDialog();
                //DO NOTHING IF THEY TRY TO MODIFY THIS ONE.
            }


        }
    }
}