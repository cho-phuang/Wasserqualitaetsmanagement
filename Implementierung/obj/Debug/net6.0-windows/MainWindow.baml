   M S B A M L   `   `   `  ����J  FProjekt Schuler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Projekt_SchulerProjekt_Schuler  5   	        Projekt_Schuler.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:Projekt_Schuler     5   	     � Title$  
MainWindow��5   	   $
��1031��6   $	��800��6*    � WindowStartupLocation$ CenterScreen=�5	   	   ���#2A2A2A.��5      � .+�5   
   �� $��Center=�6   $��Center=�60   $	��562��6M   $	��922��6Y   .+�5      �� -   5        $��	Ausloggen��5      $	��100��6)   $��30��65   $��Right=�6A   $��10q�6]   ���#FF3333
�������$��14(�5   9   6G   �� 5      $(��"Wasserqualität Ihres Schwimmbades��6   $��24(�6A   $
��Bold"�6O   ���#2D8CFF$��Center=�6v   $��10q�6�   �� 5      -   5         ��Name$ poolNameDisplay��5      $��16(�61   
�������$��Center=�6R   $��10q�6o   �� 5      -   5        $ poolLocationDisplay��5      $��16(�65   
�������$��Center=�6V   $��10q�6s   �� 5      $��pH-Wert:��6   $��16(�6(   
�������$��
10,10,10,5q�6I   �� 5      -   5         ��Name$ phValueTextBox��5      $��16(�6.   $��35��6<   
�������
���   ����#2D8CFF$��1q�6�   �� 5!      $��Temperatur (°C):��6   $��16(�60   
�������$��
10,10,10,5q�6Q   �� 5"      -   5        $ temperatureTextBox��5"      $��16(�62   $��35��6@   
�������
���   ����#2D8CFF$��1q�6�   �� 5$      $��Chlorlevel (mg/l):��6   $��16(�62   
�������$��
10,10,10,5q�6S   �� 5%      -   5        $ chlorineTextBox��5%      $��16(�6/   $��35��6=   
�������
���   ����#2D8CFF$��1q�6�   �� 5'      $��Trübung (NTU):��6   $��16(�6.   
�������$��
10,10,10,5q�6O   �� 5(      -   5        $ turbidityTextBox��5(      $��16(�60   $��35��6>   
�������
���   ����#2D8CFF$��1q�6�   �� 5+      -   5        $��Daten anzeigen��5+      $��Center=�6.   $��20,10q�6K   ���#2D8CFF
�������$��16(�6�   6�   �� 5.      $"��Eingegebene Wasserqualität:��6   $��16(�6;   $
��Bold"�6I   
�������$��Center=�6n   $��10,20q�6�   �� 52      $��	pH-Wert: ��6   $��16(�6)   
�������67   �� 53      -	   5        $ phValueDisplay��53      $��16(�60   
�������6>   �� 55      $��Temperatur (°C): ��6   $��16(�61   
�������6?   �� 56      -
   5        $ temperatureDisplay��56      $��16(�64   
�������6B   �� 58      $��Chlorlevel (mg/l): ��6   $��16(�63   
�������6A   �� 59      -   5        $ chlorineDisplay��59      $��16(�61   
�������6?   �� 5;      $��Trübung (NTU): ��6   $��16(�6/   
�������6=   �� 5<      -   5        $ turbidityDisplay��5<      $��16(�62   
�������6@   �� 5?      -   5        $ warningText��5?      $��16(�6+   
���  ��$��Center=�6J   $��20q�6g    �� 
IsReadOnly$
 True��5@       �� TextWrapping$
 Wrap=�6$    �� AcceptsReturn$
 True��68   $	��100��6M   " �� VerticalScrollBarVisibility$
 Auto=�6Z   $	��512��6}   5A      5C      5E      