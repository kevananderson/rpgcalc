using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace RPGC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.KeyUpEvent,
                new System.Windows.Input.KeyEventHandler(TextBox_KeyUp));

            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.MouseEnterEvent,
                new RoutedEventHandler(TextBox_MouseEnter));

            base.OnStartup(e);
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //get rid of the bold
            ((TextBox)sender).FontWeight = FontWeights.Normal;

            //not an enter key
            if (e.Key != System.Windows.Input.Key.Enter) return;

            //find our user controller
            UserControl usercontrol = (UserControl)(((Grid)((TextBox)sender).Parent).Parent);

            if(usercontrol is EnemyControl)
            {
                ((EnemyControl)usercontrol).EnterCard();
            }
            else if( usercontrol is PlayerControl)
            {
                ((PlayerControl)usercontrol).EnterCard();
            }
            
            e.Handled = true;
        }

        private void TextBox_MouseEnter(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Focus();
            ((TextBox)sender).SelectAll();
        }

    }
}
