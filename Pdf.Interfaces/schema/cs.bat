attrib -r *.cs

xsd requests.xsd /c /n:Pdf.Interfaces /nologo
msxsl.exe requests.xsd xsd2html.xslt > requests.html
msxsl.exe requests.xsd xsd2handlers.xslt > handlers.cs
msxsl.exe requests.xsd xsd2legos.xslt > lego_objects.template

pause