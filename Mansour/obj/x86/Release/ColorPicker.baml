   M S B A M L   `   `   `  ����   Mansour    Mansour.ColorPicker   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  اختيار لون��$	��179��$	��352��$��RightToLeft=� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=� � ShowInTaskbar .  .���  � RowDefinitions �� �� �� ��  � ColumnDefinitions �� $
��225* ��� $
��105* �.+��� -   $��SelectedColor��$��3��$��5q�
���   �$��1��
���   �$��2q���  �� Orientation$ 
Horizontal=�$��5q�.+��� $��الأحمر:��$��45���� -   $��sldRed��$	��120�� � Maximum$	 255Q� � SmallChange$ 1Q�	 �� IsSnapToTickEnabled	 . 
 � Value
 ��  �� ElementName$ txtRed�� �� Path$
 Text��� -   $��txtRed��$��50���� $ 
Horizontal=�$��5q�$��1��.+��� $��الأخضر:��$��45���� -   $��sldGreen��$	��120��$	 255Q�$ 1Q�	 . 
 �� $ txtGreen��$
 Text��� -   $��txtGreen��$��50���� $ 
Horizontal=�$��5q�$��2��.+��� $��الأزرق:��$��45���� -   $��sldBlue��$	��120��$	 255Q�$ 1Q�	 . 
 �� $ txtBlue��$
 Text��� -   $��txtBlue��$��50�� �� MaxLines$ 1���� $ 
Horizontal=�$��3��$��2��$��Right=�.+��� -	   $��	btnCancel��$��
إلغاء��$
��10,0q�$��5q��� -
   $��btnOk��$��
موافق��$
��10,0q�$��5q�