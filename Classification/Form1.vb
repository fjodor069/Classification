Public Class Form1

    ''' <summary>
    ''' 'this class implements the classification for pressure vessels
    ''' according to the Chinese Manufacturing Licensing of Pressure Vessels;
    ''' TSG R0004-2009 Supervision Regulations on Safety Technology for Stationary Pressure Vessel
    ''' fig. A1 and A2
    ''' </summary>
    ''' <remarks></remarks>
    Enum type_medium
        medium1
        medium2
    End Enum

    Enum type_category
        class0
        classI
        classII
        classIII
    End Enum

    Enum type_licensing
        A1
        A2
        D2
        D1
        unlicensed
    End Enum

    Private P As Double
    Private V As Double

    Private mediumgroup As type_medium
    Private vesselcategory As type_category
    Private licensinglevel As type_licensing


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'form load

        RadioButton1.Checked = True
        RadioButton2.Checked = False

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'category button

        'get the input
        Try
            P = Double.Parse(TextBox1.Text)
            V = Double.Parse(TextBox2.Text)
        Catch ex As Exception
            MessageBox.Show("You must enter a value")
            Return
        End Try
        'P = 1
        'V = 1
       
        If RadioButton1.Checked Then
            mediumgroup = type_medium.medium1
        Else
            mediumgroup = type_medium.medium2
        End If

        If mediumgroup = type_medium.medium1 Then
            vesselcategory = DetermineCategoryA1(P, V)
        Else
            vesselcategory = DetermineCategoryA2(P, V)
        End If

        Select Case vesselcategory

            Case type_category.class0
                licensinglevel = type_licensing.unlicensed

            Case type_category.classI
                licensinglevel = type_licensing.D1

            Case type_category.classII
                licensinglevel = type_licensing.D2

            Case type_category.classIII
                If P < 10 Then
                    licensinglevel = type_licensing.A2
                Else
                    licensinglevel = type_licensing.A1
                End If


        End Select

        PrintResults()

        DrawPicture()

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged

        Dim rb As RadioButton = TryCast(sender, RadioButton)

        If rb Is Nothing Then Return

        If RadioButton1.Checked Then
            RadioButton2.Checked = False
        End If
        If RadioButton2.Checked Then
            RadioButton1.Checked = False
        End If


    End Sub

    'teken de grafiek in de picturebox
    Private Sub DrawPicture()


        Dim g As Graphics = PictureBox1.CreateGraphics()
        Dim rect As Rectangle
        Dim margin As Integer = 20
        Dim numX As Integer = 10
        Dim numY As Integer = 4

        ' g.DrawLine(Pens.Black, 10, 10, 20, 20)
        rect = New Rectangle(0 + margin, 0 + margin, PictureBox1.Width - margin * 2, PictureBox1.Height - margin * 2)

        g.DrawRectangle(Pens.Blue, rect)

        'zet de origin op een ander punt
        g.TranslateTransform(margin, PictureBox1.Height - margin)

        Dim p1 As New Pen(Color.Black, 2)
        'teken de x -as 
        Dim distX As Integer = PictureBox1.Width - margin * 2
        Debug.Print("distX = " & distX)

        g.DrawLine(p1, 0, 0, distX, 0)

        'teken de y -as
        Dim distY = PictureBox1.Height - margin * 2
        Debug.Print("distY = " & distY)
        g.DrawLine(p1, 0, 0, 0, -distY)

        'teken major gridlines verticaal
        For i As Integer = 0 To numX - 1
            Dim xdist As Integer = i / numX * distX
            g.DrawLine(Pens.Blue, xdist, 0, xdist, -distY)
        Next

        'teken minor gridlines verticaal
        For i As Integer = 0 To numX - 1
            Dim xdist As Integer = i / numX * distX
            For j As Integer = 1 To 9

                Dim logdist As Integer = Math.Log10(j) * (distX / numX) + xdist
                g.DrawLine(Pens.Black, logdist, 0, logdist, -distY)

            Next
        Next

        'teken major gridlines horizontaal
        For i As Integer = 0 To numY - 1
            Dim ydist As Integer = i / numY * distY
            g.DrawLine(Pens.Blue, 0, -ydist, distX, -ydist)
        Next

        'teken minor gridlines horizontaal
        For i As Integer = 0 To numY - 1
            Dim ydist As Integer = i / numY * distY
            For j As Integer = 1 To 9

                Dim logdist As Integer = Math.Log10(j) * (distY / numY) + ydist
                g.DrawLine(Pens.Black, 0, -logdist, distX, -logdist)

            Next
        Next

        'teken het punt P,V


        Dim myPoint As New Point

        myPoint.X = Int(Math.Log10(V) / (numX - 1) * distX)
        myPoint.Y = -Int(Math.Log10(P) / (numY - 1) * distY)
        Debug.Print(myPoint.ToString)

        g.FillEllipse(Brushes.Crimson, myPoint.X, myPoint.Y, 5, 5)
       
    End Sub


    Private Sub PrintResults()

        Dim s As String

        RichTextBox1.Clear()
        RichTextBox1.AppendText("Pressure P = " & P & vbCrLf)
        RichTextBox1.AppendText("Volume V = " & V & vbCrLf)

        If mediumgroup = type_medium.medium1 Then
            s = "medium 1"
        Else
            s = "medium 2"
        End If
        RichTextBox1.AppendText("Medium group = " & s & vbCrLf)

        RichTextBox1.AppendText("Vessel category = " & vesselcategory.ToString & vbCrLf)

        RichTextBox1.AppendText("Licensing level = " & licensinglevel.ToString & vbCrLf)
        'Select Case licensinglevel

        '    Case type_licensing.A1
        '        RichTextBox1.AppendText(licensinglevel.ToString & vbCrLf)
        '    Case type_licensing.A2
        '        RichTextBox1.AppendText("group = " & s & vbCrLf)
        '    Case type_licensing.D1
        '        RichTextBox1.AppendText("Medium group = " & s & vbCrLf)
        '    Case type_licensing.D2
        '        RichTextBox1.AppendText("Medium group = " & s & vbCrLf)



        'End Select



    End Sub

    Private Function DetermineCategoryA1(ByVal P As Double, ByVal V As Double) As type_category

        Dim PV As Double
        Dim myCategory As type_category

        PV = P * V

        'for medium group 1
        If (PV < 2.5 And V < 25) Or (P < 0.1) Then
            myCategory = type_category.class0

        Else
            If (V < 25) Then
                myCategory = type_category.classI
            Else
                If (P < 10 And V < 5000.0) Then
                    myCategory = type_category.classII
                Else
                    If (P < 10 And PV < 50000.0 And V < 31250) Then
                        myCategory = type_category.classII
                    Else
                        If (P < 1.6 And V < 600000.0) Then
                            myCategory = type_category.classII
                        Else
                            If (P < 1.6 And PV < 1000000.0 And V < 10000000.0) Then
                                myCategory = type_category.classII
                            Else
                                myCategory = type_category.classIII
                            End If
                        End If
                    End If

                End If
            End If
        End If


        Return myCategory


    End Function

    Private Function DetermineCategoryA2(ByVal P As Double, ByVal V As Double) As type_category

        Dim PV As Double
        Dim myCategory As type_category

        PV = P * V

        'for medium group 2
        If (PV < 2.5 And V < 25) Or (P < 0.1) Then
            myCategory = type_category.class0

        Else
            If (V < 25) Then
                myCategory = type_category.classI
            Else
                If (P < 1.6 And V < 3125000.0) Then
                    myCategory = type_category.classI
                Else
                    If (P < 1.6 And PV < 5000000.0 And V < 500000.0) Then
                        myCategory = type_category.classI
                    Else
                        If (P < 10 And V < 50000.0) Then
                            myCategory = type_category.classII
                        Else
                            If (P < 10 And PV < 50000.0 And V < 312500) Then
                                myCategory = type_category.classII
                            Else
                                myCategory = type_category.classIII
                            End If
                        End If
                    End If

                End If
            End If
        End If

        Return myCategory

    End Function

End Class
