   M S B A M L   `   `   `  ����   Mansour    Mansour.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$7  1منصور - محلل النصوص العربية��$	��628��$	��693��$��RightToLeft=����	#FFEBEBEB � WindowStyle$ ThreeDBorderWindow=� � WindowStartupLocation$ CenterScreen=� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$
 Open��  -   $
 Save��  -   $ SaveAs��  -   $	 New��  -   $ Close��  -   $
 Help��.��]� .+�o� $	��Top=�.@�m� $��_ملف��.@�m� $��	_جديد�� m� Command$ ApplicationCommands.New��m� $��_فتح ملف نصي��$ ApplicationCommands.Open���� m� $��حفظ المدخلات ��$ ApplicationCommands.Save��m� $%��حفظ المدخلات كـ...��$  ApplicationCommands.SaveAs��m� -   $��mnuExportOutput��$#��حفظ النتائج كـ...���� m� $��إ_غلاق��$ ApplicationCommands.Close��m� $��ت_حرير��.@�m� $ ApplicationCommands.Undo��$��
تراجع��m� $ ApplicationCommands.Redo��$��
إعادة���� m� $ ApplicationCommands.Copy��$��نسخ��m� $ ApplicationCommands.Paste��$��لصق���� m� -	   $��mnuCopyOutput��$��نسخ المخرجات��m� -
   $��mnuPasteInput��$��لصق المدخلات���� m� -   $��
mnuOptions��$��خيارات...��m� $��ت_طوير��.@�m� -   $��mnuAddForeignWord��$%��أضف كلمة أعجمية...��m� $��م_ساعدة��.@�m� $ ApplicationCommands.Help��$��المساعدة...��m� -   $��mnuAbout��$��عن البرنامج...���� -   $��	StatusBar��$��Bottom=�$��23��.@��� -   $��stsWords��$��عدد الكلمات: 0���� -   $��stsSentences��$��عدد الجمل: 0���� -   $��stsCorrections��$��التصحيحات: 0���� -   $��
stsPercent��$ ��نسبة المصحح: 0 %���� -   $��stsFuzzy��$��غير المفسر: 0���� -   $��	stsStatus��$��الحالة: جاهز.��� -   $��Progress��$	��100�� � Value$ 0Q�$��15��$y�	Collapsed=��  � RowDefinitions �� �� .+�� $��0�� � ColumnDefinitions �� �� $	��130 �.+��� $��0��$*��$النص المطلوب تحليله��$��3q�.���� -   $��txtOriginal��$��0,5,0,0q�	 �� TextWrapping$
	 Wrap=�"
 �� VerticalScrollBarVisibility$
 Visible=�$��Right=�$��20(� �� TextAlignment$ Justify=�$��Arial)� �� AcceptsReturn .  �� 
AcceptsTab . �� $��1��$��5q�.+��� -   $��btnPaste��$ ��لصق من الحافظة��$	��5,2q�$��30���� -   $��btnRemoveDiacrits��$��إزالة التشكيل��$	��5,2q�$��30���� -   $��	chkIgnore��$,��&تجاهل التشكيل المسبق��$	��5,2q��� -   $��chkQuick��$��تحليل سريع��$	��5,2q� d� 	IsChecked$
 TrueY��� -   $��chkLearning��$��نمط التعلم��$	��5,2q�$
 TrueY��� -   $��btnStart��$��بدء التحليل��$	��5,2q�$��30��� $��1�� �� �� $	��130 � �� �� $��35 �.+��� $��0��$��المخرجات��$��3q�.���� -   $��txtOut��$��0��$��0,5,0,0q�$	��2,5q�$
	 Wrap=�$
 Visible=�$��Right=�$��20(�$ Justify=�$��Arial)� �� 
IsReadOnly . ���	#FFFFFDEF . � $��1��.+��� -   $��txtAnalysis��$��التحليل: ��$��15(�$
��auto��$��5q��� -   $��
btnDetails��$��تفاصيل...��$	��5,0q�$��Right=�{�.  $��5q��� $��5q�$��1��$��2��.+��� -    $��btnCopy��$"��نسخ إلى الحافظة��$	��5,0q�$��30���� $��المقترحات :���� -!   $��lstPossibilities��$	��130��$��5q�$��Arial)�$��18(�|�.  �� -"   $��	btnModify��$��
تعديل��$��Right=�$��60��$	��5,0q�{�.  |�.  