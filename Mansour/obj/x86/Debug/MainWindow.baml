   M S B A M L   `   `   `  ����  
AbgadHawaz    Mansour.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  ABGAD HAWAZ��5   	   $	��695��6   $
��1219��6*   $��RightToLeft=�67   ���	#FFEBEBEB � WindowStyle$ ThreeDBorderWindow=�6j    � WindowStartupLocation$ CenterScreen=�6�    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$
 Open��5      6I     5   
   -   5        $
 Save��5      6I     5   
   -   5        $ SaveAs��5      6M     5	   
   -   5        $	 New��5	      6G     5
   
   -   5        $ Close��5
      6K     5   
   -   5        $
 Help��5      6J   .��5      ]� .+�5   
   o� $	��Top=�6   .@�5      m� $��_File��6   .@�5      m� $
��_New��6    m� Command$ ApplicationCommands.New��6)   m� 5      $��
_Open file��6   $ ApplicationCommands.Open��6/   �� 5      m� 5      $��Save input as ��6   $ ApplicationCommands.Save��63   m� 5      $��save output as...��6   $  ApplicationCommands.SaveAs��66   m� 5      -   5        $��mnuExportOutput��5   T   $��save result as...��6   6T   �� 5      m� 5      $��close��6   $ ApplicationCommands.Close��6*   5      m� 5      $
��Edit��6   .@�5      m� $ ApplicationCommands.Undo��6   $
��Undo��6>   m� 5      $ ApplicationCommands.Redo��6   $
��redo��6>   �� 5      m� 5      $ ApplicationCommands.Copy��6   $
��copy��6>   m� 5      $ ApplicationCommands.Paste��6   $��paste��6?   �� 5      m� 5       -	   5        $��mnuCopyOutput��5    0   $��copy output��6   6E   m� 5!      -
   5        $��mnuPasteInput��5!   /   $��
copy input��6   6D   �� 5"      m� 5#      -   5        $��
mnuOptions��5#   /   $��
options...��6   6A   5$      m� 5%      $��Develope��6   .@�5&      m� -   5        $��mnuAddForeignWord��5&   T   $��ADD New Word...��6   6T   5(      m� 5)      $
��Help��6   .@�5*      m� $ ApplicationCommands.Help��6   $��Help...��6>   m� 5+      -   5        $��mnuAbout��5+   -   $��About...��6   6=   5,      5-      �� 5.   
   -   5        $��	StatusBar��5.   8   $��Bottom=�6   $��23��6,   .@�5/      �� -   5        $��stsWords��5/      $��words number: 0��6(   �� 50      -   5        $��stsSentences��50      $��sentence number: 0��6,   �� 51      -   5        $��stsCorrections��51      $��Corrected: 0��6.   �� 52      -   5        $��
stsPercent��52      $!��correctness precentage: 0 %��6*   �� 53      -   5        $��stsFuzzy��53      $��Unexplained: 0��6(   �� 54      -   5        $��	stsStatus��54      $��state:ready.��6)   � 55      -   5        $��Progress��55      $	��100��6*    � Value$ 0Q�66   $��15��6@   $y�	Collapsed=�6L   56      � 57   
    � RowDefinitions 58      �� 59      �� 5:      .+�5<      � $��0��6    � ColumnDefinitions 5=      �� 5>      �� 5?      $	��130 �6'   .+�5A      �� $��0��6   $��The desired text analysis��6+   $��3q�6N   .��5B      �� -   5        $��txtOriginal��5B      $��0,5,0,0q�62   	 �� TextWrapping$
	 Wrap=�6C   "
 �� VerticalScrollBarVisibility$
 Visible=�6W   $��Right=�6}   $��20(�6�    �� TextAlignment$ Justify=�6�   $��Arial)�6�    �� AcceptsReturn .  �� 
AcceptsTab . 6   5C      �� 5D      $��1��6   $��1q�6-   .+�5E      �� -   5        $��btnPaste��5E      $��Paste from Clipboard��6.   $	��5,2q�6M   $��49��6Z   $	��118��6}   �� 5F      -   5        $��btnRemoveDiacrits��5F   Q   $��remove diacritic��6   $	��5,2q�68   $��43��6E   6j   �� 5G      -   5        $��btnStart��5G   P   $��Start analysis��6   $
��5,15q�66   $��66��6D   $	��110��6w   5H      5I      � 5J      $��1��6    5K      �� 5L      �� 5M      $	��130 �6'    5O      �� 5P      �� 5Q      $��35 �6$   .+�5T      �� $��Output��6   $��3,5,3,0q�6+   .��5U      �� -   5        $��txtOut��5U      $��0��6,   $��	0,5,0,200q�6<   $	��2,5q�6O   $
	 Wrap=�6]   $
 Visible=�6q   $��Right=�6�   $��20(�6�   $ Justify=�6�   $��Arial)�6�    �� 
IsReadOnly . ���	#FFFFFDEF . $��51��6\  $
��1044��6h  5V      � 5W      $��1��6   .+�5X      �� -   5        $��txtAnalysis��5X   B   $��: Analysis ��6    $��15(�64   $
��auto��6U   $��5q�6b   �� 5Y      -   5        $��
btnDetails��5Y   z   $��... Details��6   $	��5,0q�63   $��Right=�6A   {�.  $��5q�6o   6�   5Z      �� 5[      $��5q�6   $��1��6(   $��2��68   .+�5\      �� -   5        $��btnCopy��5\   R   $��Copy to Clipboard��6   $	��5,0q�69   $��30��6F   6a   �� 5]      $�� : suggestions ��6   �� 5^      -   5        $��lstPossibilities��5^      $	��130��66   $��5q�6C   $��Arial)�6N   $��18(�6a   |�.  6�   �� 5_      -   5        $��	btnModify��5_   b   $��Modify��6   $��Right=�6.   $��60��6J   $	��5,0q�6U   {�.  |�.  6�   5`      &   System.Windows.Controls.DataGrid  5a      -    5        $��	dataGrid1��5a   q      AutoGenerateColumns .  $	��172��67   $
��Left=�6D   $��6,84,0,0q�6_   $	��Top=�6�   $
��1050��6�   5b      5c      5e      5f      