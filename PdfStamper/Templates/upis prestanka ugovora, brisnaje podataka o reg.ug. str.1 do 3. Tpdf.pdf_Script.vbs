Set theDoc = CreateObject("ABCpdf9.Doc")
theDoc.Read "upis prestanka ugovora, brisnaje podataka o reg.ug. str.1 do 3. Tpdf.pdf"
theDoc.AddFont "Helvetica-Bold"
theDoc.FontSize=16
theDoc.Rect.Pin=1

Dim theIDs, theList
theIDs = theDoc.GetInfo(theDoc.Root, "Field IDs")
theList = Split(theIDs, ",")

For Each id In theList
    theDoc.Page = theDoc.GetInfo(id, "Page")
    theDoc.Rect.String = theDoc.GetInfo(id, "Rect")
    theDoc.Color.String = "240 240 255"
    theDoc.FillRect()
    theDoc.Rect.Height = 16
    theDoc.Rect.Width = 150
    theDoc.Color.String = "220 0 0"
    theDoc.AddText(theDoc.GetInfo(id, "Name"))
    theDoc.Delete(id)
Next

theDoc.Save "Template_upis prestanka ugovora, brisnaje podataka o reg.ug. str.1 do 3. Tpdf.pdf"
theDoc.Clear
MsgBox "Finished"