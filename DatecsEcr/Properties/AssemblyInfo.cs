﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Управление общими сведениями о сборке осуществляется с помощью 
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("DatecsEcr")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DatecsEcr")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Параметр ComVisible со значением FALSE делает типы в сборке невидимыми 
// для COM-компонентов.  Если требуется обратиться к типу в этой сборке через 
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(true)]
[assembly: AssemblyKeyFile("DatecsEcr.snk")]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("73edd917-7d2b-4784-b4e3-0c8aaf90610d")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии 
//      Номер построения
//      Редакция
//
// Можно задать все значения или принять номер построения и номер редакции по умолчанию, 
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.3")]
[assembly: AssemblyFileVersion("1.0.0.3")]

// 1.0.0.2. Реализованы все функции. Запись логов и ошибок. Изменения значений свойств при поступлении данных от РРО.

// 1.0.0.3. В лог добавлена запись милисекунд и данных получаемых от принтера. Статус записывается в свойство 
// в виде строки из байт в формате ХХ-ХХ-ХХ-ХХ-ХХ-ХХ. 