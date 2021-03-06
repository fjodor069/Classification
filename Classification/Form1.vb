﻿Public Class Form1

    ''' <summary>
    ''' 'this class implements the classification for pressure vessels
    ''' according to the Chinese Manufacturing Licensing of Pressure Vessels;
    ''' TSG R0004-2009 Supervision Regulations on Safety Technology for Stationary Pressure Vessel
    ''' fig. A1 and A2
    ''' update for TSG 21-2016 
    ''' fig A1 and A2 are changed
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

    Dim numX As Integer = 8             ' number of divisions on X-axis
    Dim numY As Integer = 5
    Dim gmargin As Integer = 30

    Dim bValid As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'form load

        RadioButton1.Checked = True
        RadioButton2.Checked = False
        mediumgroup = type_medium.medium1
        PictureBox1.Invalidate()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'category button

        'get the input
        'limit is P <= 350 MPa and V <= 1E6 m3
        bValid = False
        Try
            P = Double.Parse(TextBox1.Text)
            V = Double.Parse(TextBox2.Text)
        Catch ex As Exception
            MessageBox.Show("You must enter a value")
            Return
        End Try
        'limit is P <= 350 and V <= 1E6
        If (P > 350) Then
            MessageBox.Show("Pressure > 350 is out of limits")
            Return
        End If
        If (V > 1000000.0) Then
            MessageBox.Show("Volume > 1E6 is out of limits")
            Return
        End If
        'also limit for negative values
        If (P < 0.01) Then
            MessageBox.Show("Pressure < 1e-2 is out of limits")
            Return
        End If
        If (V < 0.01) Then
            MessageBox.Show("Volume < 1e-2 is out of limits")
            Return
        End If





        'we have a valid point , now redraw the picturebox
        bValid = True
        PictureBox1.Invalidate()


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

        If RadioButton1.Checked Then
            mediumgroup = type_medium.medium1
        ElseIf RadioButton2.Checked Then
            mediumgroup = type_medium.medium2
        End If

        PictureBox1.Invalidate()

    End Sub

    'teken de grafiek in de picturebox
    Private Sub DrawPicture(g As Graphics)



        'text font for legenda
        Dim TextFont As New System.Drawing.Font("Arial", 8, FontStyle.Regular)
        Dim TextBrush As New System.Drawing.SolidBrush(Color.Blue)
        Dim TextFont1 As New System.Drawing.Font("Arial", 10, FontStyle.Bold)

        Dim TextBrush1 As New System.Drawing.SolidBrush(Color.BlueViolet)
        Dim myMayorPen As New Pen(Brushes.Black, 1)
        Dim myMinorPen As New Pen(Brushes.Gray, 1)

        'clear the entire picture to remove points
        g.Clear(Color.White)
       

        'zet de origin op een ander punt
        g.TranslateTransform(gmargin, PictureBox1.Height - gmargin)

        Dim p1 As New Pen(Color.Black, 2)
        'teken de x -as 
        Dim distX As Integer = PictureBox1.Width - gmargin * 2
        ' Debug.Print("distX = " & distX)

        g.DrawLine(p1, 0, 0, distX, 0)

        'teken de y -as
        Dim distY = PictureBox1.Height - gmargin * 2
        'Debug.Print("distY = " & distY)
        g.DrawLine(p1, 0, 0, 0, -distY)

        'teken major gridlines verticaal
        'en de legenda op de x as
        For i As Integer = 0 To numX
            Dim xdist As Integer = i / numX * distX
            g.DrawLine(myMayorPen, xdist, 0, xdist, -distY)
            g.DrawString("1e" & Format(i - 2, "0"), TextFont, TextBrush, xdist - 5, 0)
            'Debug.Print(i)
        Next
        g.DrawString("Volume (m3)", TextFont, TextBrush, distX / 2, 15)

        'teken minor gridlines verticaal
        For i As Integer = 0 To numX - 1
            Dim xdist As Integer = i / numX * distX
            For j As Integer = 2 To 9

                Dim logdist As Integer = Math.Log10(j) * (distX / numX) + xdist
                g.DrawLine(myMinorPen, logdist, 0, logdist, -distY)

            Next
        Next

        'teken major gridlines horizontaal
        'en schaal op de y as
        For i As Integer = 0 To numY
            Dim ydist As Integer = i / numY * distY
            g.DrawLine(myMayorPen, 0, -ydist, distX, -ydist)

            g.DrawString("1e" & Format(i - 2, "0"), TextFont, TextBrush, 0 - gmargin + 5, -ydist - 8)

        Next

        g.DrawString("Pressure (MPa)", TextFont, TextBrush, -gmargin + 5, -distY - gmargin)


        'teken minor gridlines horizontaal
        For i As Integer = 0 To numY - 1
            Dim ydist As Integer = i / numY * distY
            For j As Integer = 2 To 9

                Dim logdist As Integer = Math.Log10(j) * (distY / numY) + ydist
                g.DrawLine(myMinorPen, 0, -logdist, distX, -logdist)

            Next
        Next

        'teken region borders 
       

        If (mediumgroup = type_medium.medium1) Then

            DrawBorderLine(g, 0.03, 0.1, 1000000.0, 0.1)
            DrawBorderLine(g, 0.03, 0.1, 0.03, 350.0)

            DrawBorderLine(g, 0.03, 10, 5, 10)
            DrawBorderLine(g, 5, 10, 31.25, 1.6)
            DrawBorderLine(g, 31.25, 1.6, 625, 1.6)
            DrawBorderLine(g, 625, 1.6, 10000.0, 0.1)


        ElseIf (mediumgroup = type_medium.medium2) Then

            DrawBorderLine(g, 0.03, 0.1, 1000000.0, 0.1)
            DrawBorderLine(g, 0.03, 0.1, 0.03, 350)

            DrawBorderLine(g, 0.03, 10, 50, 10)
            DrawBorderLine(g, 0.03, 1.6, 3125, 1.6)

            DrawBorderLine(g, 50, 10, 312.5, 1.6)
            DrawBorderLine(g, 3125, 1.6, 50000, 0.1)



        End If

        'teken tekst in regions
        If (mediumgroup = type_medium.medium2) Then
            g.DrawString(type_category.classI.ToString, TextFont1, TextBrush1, 70, -100)
        End If

        g.DrawString(type_category.classII.ToString, TextFont1, TextBrush1, 70, -140)
        g.DrawString(type_category.classIII.ToString, TextFont1, TextBrush1, 300, -220)

        'if there is a valid point we can draw it 
        If bValid Then
            DrawPoint(g, P, V)
        End If


    End Sub
    Private Sub DrawPoint(g As Graphics, ByVal P As Double, ByVal V As Double)

        'teken het punt P,V-----------------
        Dim distX As Integer = PictureBox1.Width - gmargin * 2
        Dim distY = PictureBox1.Height - gmargin * 2

        Dim myPoint As New Point

        myPoint.X = Int((Math.Log10(V) + 2) * distX / numX)
        myPoint.Y = -Int((Math.Log10(P) + 2) * distY / numY)
        '  Debug.Print(myPoint.ToString)

        Dim pointsize As Integer = 5
        myPoint.X = Int(myPoint.X - pointsize / 2)
        myPoint.Y = Int(myPoint.Y - pointsize / 2)


        g.FillEllipse(Brushes.Crimson, myPoint.X, myPoint.Y, pointsize, pointsize)

    End Sub

    Private Sub DrawBorderLine(g As Graphics, X1 As Double, Y1 As Double, X2 As Double, Y2 As Double)

        Dim distX As Integer = PictureBox1.Width - gmargin * 2
        Dim distY = PictureBox1.Height - gmargin * 2

        Dim P1, P2 As Point

        P1.X = Int((Math.Log10(X1) + 2) * distX / numX)
        P1.Y = -Int((Math.Log10(Y1) + 2) * distY / numY)

        P2.X = Int((Math.Log10(X2) + 2) * distX / numX)
        P2.Y = -Int((Math.Log10(Y2) + 2) * distY / numY)

        Dim myPen As New Pen(Brushes.Black, 2)

        g.DrawLine(myPen, P1, P2)

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




    End Sub

    Private Function DetermineCategoryA1(ByVal P As Double, ByVal V As Double) As type_category

        Dim PV As Double
        Dim myCategory As type_category

        PV = P * V

        'for medium group 1
        If (V < 0.03) Or (P < 0.1) Then
            myCategory = type_category.class0

        Else
            If (P < 10 And PV < 50.0) Then
                myCategory = type_category.classII
            Else
                If (P < 1.6 And PV < 1000.0) Then
                    myCategory = type_category.classII
                Else
                    myCategory = type_category.classIII

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
        If (V < 0.03) Or (P < 0.1) Then
            myCategory = type_category.class0

        Else
            If (P < 1.6 And PV < 5000) Then
                myCategory = type_category.classI
            Else
                If (P < 10 And PV < 500) Then
                    myCategory = type_category.classII
                Else
                    myCategory = type_category.classIII
                End If

            End If
        End If

        Return myCategory

    End Function

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint

        DrawPicture(e.Graphics)

    End Sub

   
End Class
