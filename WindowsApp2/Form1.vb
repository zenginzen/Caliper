Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Public Class Form1
    Private Const WM_HOTKEY As Integer = &H312
    Private Const MOD_CONTROL As Integer = &H2
    Private Const VK_M As Integer = &H45
    Private startPoint As Point
    Private squareSize As Size
    Private isDrawing As Boolean
    Public Squares As List(Of Rectangle) = New List(Of Rectangle)
    Public listallsquares As List(Of AllSquares) = New List(Of AllSquares)
    Public workingsquare As Integer
    Public ratio As Decimal
    Public modifiedratio As Decimal
    Dim working As Boolean = False
    Dim moving As Boolean = False
    Dim calibrating As Boolean = False
    Public calibrationwidth As Integer
    Dim copies As Integer = 0
    Dim screens As Screen() = Screen.AllScreens
    Dim screensaveposition As Integer = 0
    Public Zoom As Integer = 100


    <DllImport("user32.dll")>
    Private Shared Function RegisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function UnregisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer) As Boolean
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)


        If m.Msg = WM_HOTKEY Then
            ' Check if the hotkey ID matches the assigned ID (e.g., 1)
            If m.WParam.ToInt32() = 1 Then
                ' Maximize the form
                If Me.WindowState = FormWindowState.Maximized = True Then
                    Me.WindowState = FormWindowState.Minimized
                Else
                    Me.WindowState = FormWindowState.Maximized
                    Invalidate()

                End If
            End If
        Else
            MyBase.WndProc(m)
        End If

    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If working = True Then
            moving = True

        Else
            startPoint = e.Location
            squareSize = Size.Empty
            isDrawing = True
            moving = False

        End If
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        MyBase.OnMouseMove(e)


        If working = True Then
            If moving = True Then

                Dim ns As New Rectangle
                ns.Location = e.Location
                ns.Size = listallsquares(workingsquare).Square.Size
                listallsquares(workingsquare).Square = ns
                Refresh()

            End If
        Else
            If isDrawing Then
                Dim rect As New Rectangle()
                If e.X >= startPoint.X AndAlso e.Y >= startPoint.Y Then
                    rect.Location = startPoint
                    rect.Size = New Size(e.X - startPoint.X, e.Y - startPoint.Y)
                ElseIf e.X < startPoint.X AndAlso e.Y >= startPoint.Y Then
                    rect.Location = New Point(e.X, startPoint.Y)
                    rect.Size = New Size(startPoint.X - e.X, e.Y - startPoint.Y)
                ElseIf e.X >= startPoint.X AndAlso e.Y < startPoint.Y Then
                    rect.Location = New Point(startPoint.X, e.Y)
                    rect.Size = New Size(e.X - startPoint.X, startPoint.Y - e.Y)
                Else ' e.X < startPoint.X AndAlso e.Y < startPoint.Y
                    rect.Location = e.Location
                    rect.Size = New Size(startPoint.X - e.X, startPoint.Y - e.Y)
                End If
                listallsquares(workingsquare).Square = rect
                Refresh()


            End If

        End If

    End Sub



    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)

        MyBase.OnMouseUp(e)
        isDrawing = False
        working = True
        moving = False


        If calibrating = True Then
            calibrationwidth = listallsquares(workingsquare).Square.Width
            For Each x As Control In Me.Controls
                If TypeOf x Is Timing Then
                    Dim y As Timing = x
                    y.Label5.Text = "Box Size: " & calibrationwidth
                End If
            Next

        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

            Using g As Graphics = e.Graphics

            For Each r As AllSquares In listallsquares



                Dim rectangle As New Rectangle(r.Square.Location, r.Square.Size)

                g.DrawRectangle(r.Pen, rectangle)

                drawlabel(e.Graphics, rectangle)


                Dim brush As New SolidBrush(r.Pen.Color)
                Dim Font As New Font("Arial", 8, FontStyle.Bold)
                Dim Text As String = (rectangle.Width * modifiedratio).ToString("N2") & "ms"

                Dim backbrush As New SolidBrush(Color.Black)
                Dim rectWidth As Integer = CInt(g.MeasureString(Text, Font).Width) + 10
                Dim rectHeight As Integer = CInt(g.MeasureString(Text, Font).Height) + 10
                Dim rectX As Integer = Rectangle.Left - 5
                Dim rectY As Integer = Rectangle.Top - 30
                g.FillRectangle(backbrush, rectX, rectY, rectWidth, rectHeight)
                g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                g.DrawString(Text, Font, brush, rectangle.Left, rectangle.Top - 25)

                Dim Notatebrush As New SolidBrush(r.Pen.Color)
                Dim NotateFont As New Font("Arial", 8, FontStyle.Bold)
                Dim NotateText As String = r.Notation

                Dim Notatebackbrush As New SolidBrush(Color.Black)
                Dim NotaterectWidth As Integer = CInt(g.MeasureString(NotateText, NotateFont).Width) + 10
                Dim NotaterectHeight As Integer = CInt(g.MeasureString(NotateText, NotateFont).Height) + 5
                Dim NotaterectX As Integer = r.Square.Left
                Dim NotaterectY As Integer = r.Square.Top + r.Square.Height + 10
                g.FillRectangle(Notatebackbrush, NotaterectX, NotaterectY, NotaterectWidth, NotaterectHeight)

                Dim boarderpoint As New Point(NotaterectX, NotaterectY)
                Dim boardersize As New Size(NotaterectWidth, NotaterectHeight)
                Dim Boarderrectangle As New Rectangle(boarderpoint, boardersize)

                g.DrawRectangle(r.Pen, Boarderrectangle)
                g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
                g.DrawString(NotateText, NotateFont, Notatebrush, r.Square.Left + 5, r.Square.Top + r.Square.Height + 13)
            Next


        End Using


    End Sub

    Private Function drawlabel(graphics As Graphics, rectangle As Rectangle)

    End Function

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        End
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

        listallsquares(workingsquare).Copies = listallsquares(workingsquare).Copies + 1
        For x = 1 To listallsquares(workingsquare).Copies + 1

            Using g As Graphics = Me.CreateGraphics

                Dim i As New Point
                i.X = listallsquares(workingsquare).Square.X + listallsquares(workingsquare).Square.Width * (listallsquares(workingsquare).Copies)
                i.Y = listallsquares(workingsquare).Square.Y
                Dim rectangle As New Rectangle(i, listallsquares(workingsquare).Square.Size)

                g.DrawRectangle(listallsquares(workingsquare).Pen, rectangle)

            End Using

        Next


    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click

        If listallsquares(workingsquare).Square.Size.Width = 0 Then
            For Each x As AllSquares In listallsquares
                Dim tempSize As New Size(0, 0)
                Dim Templocation As New Point(99999, 99999)
                Dim temprectangle As New Rectangle(Templocation, tempSize)
                x.Square = temprectangle
                x.Copies = 0
                x.Notation = ""
            Next
        Else
            Dim temp As New Size(0, 0)
            Dim loc As New Point(99999, 99999)
            Dim temprect As New Rectangle(loc, temp)
            listallsquares(workingsquare).Square = temprect
            listallsquares(workingsquare).Copies = 0
            listallsquares(workingsquare).Notation = ""
        End If


        Refresh()

        working = False
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Dim x As New Timing
        x.Left = 0
        x.Top = 30
        x.parentform = Me
        x.Name = "calibrationform"
        Me.Controls.Add(x)
        calibrating = True
    End Sub




    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load




        For i As Integer = 0 To screens.Length - 1
            Dim screen As Screen = screens(i)

        Next
        Dim loadscreen As Integer
        Try
            Dim sr As StreamReader = New StreamReader("ScreenPosition.cfg")
            loadscreen = sr.ReadLine

            sr.Close()
        Catch ex As Exception
            loadscreen = 0
        End Try

        Dim currentScreen As Screen = Screen.FromHandle(Me.Handle)

        If screens.Count = 1 Then
            ToolStrip1.Items.Remove(Screen3)
            ToolStrip1.Items.Remove(Screen2)
            Screen1.Image = My.Resources._1Green2

        End If

        If screens.Count = 2 Then
            ToolStrip1.Items.Remove(Screen3)

        End If

        Dim selectedIndex As Integer = loadscreen
        If selectedIndex >= 0 AndAlso selectedIndex < screens.Length Then
            If selectedIndex = 0 Then
                Screen1.Image = My.Resources._1Green2
                Try
                    Screen1.ToolTipText = "Screen 1:" & screens(0).Bounds.ToString
                    Screen2.Image = My.Resources._2Red2
                    Screen2.ToolTipText = "Screen 2:" & screens(1).Bounds.ToString
                    Screen3.Image = My.Resources._3Red2
                    Screen3.ToolTipText = "Screen 3:" & screens(2).Bounds.ToString
                Catch ex As Exception

                End Try
            End If

            If selectedIndex = 1 Then
                Screen2.Image = My.Resources._2Green2
                Try
                    Screen1.ToolTipText = "Screen 1:" & screens(0).Bounds.ToString
                    Screen1.Image = My.Resources._1Red2
                    Screen2.ToolTipText = "Screen 2:" & screens(1).Bounds.ToString
                    Screen3.Image = My.Resources._3Red2
                    Screen3.ToolTipText = "Screen 3:" & screens(2).Bounds.ToString
                Catch ex As Exception

                End Try
            End If

            If selectedIndex = 2 Then
                Screen1.ToolTipText = "Screen 1:" & screens(0).Bounds.ToString
                Screen3.Image = My.Resources._3Green2
                Try
                    Screen1.Image = My.Resources._1Red2
                    Screen2.Image = My.Resources._2Red2
                    Screen2.ToolTipText = "Screen 2:" & screens(1).Bounds.ToString
                    Screen3.ToolTipText = "Screen 3:" & screens(2).Bounds.ToString
                Catch ex As Exception

                End Try
            End If
            ' Change the form's screen to the selected screen
            Me.WindowState = FormWindowState.Normal
            Dim selectedScreen As Screen = screens(selectedIndex)
            Me.Location = selectedScreen.Bounds.Location
            Me.Size = selectedScreen.Bounds.Size
            Me.StartPosition = FormStartPosition.Manual ' Set form position manually
            Me.Location = selectedScreen.Bounds.Location ' Set the form's Screen property
        End If



        RegisterHotKey(Me.Handle, 1, MOD_CONTROL, VK_M)

        workingsquare = 0
        Try
            Dim sr As StreamReader = New StreamReader("TimingCalibration.cfg")
            ratio = sr.ReadLine
            sr.Close()
        Catch ex As Exception
            ratio = 1
        End Try

        modifiedratio = (ratio / 100) * Zoom
        AllSquares.LoadSquares()
        Me.DoubleBuffered = False
    End Sub



    Private Sub ToolStripComboBox1_TextChanged(sender As Object, e As EventArgs)
        modifiedratio = (ratio) / (Zoom / 100)
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.Control AndAlso e.KeyCode = Keys.M Then
            If Me.WindowState = FormWindowState.Maximized = True Then
                Me.WindowState = FormWindowState.Minimized
            End If
        End If
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        UnregisterHotKey(Me.Handle, 1)
    End Sub



    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Dim x As New Notation
        x.Left = (Me.Width / 2) - (x.Width / 2)
        x.Top = Me.ToolStrip1.Height
        x.Measurement = listallsquares(workingsquare)
        x.ParentForm = Me
        Me.Controls.Add(x)
    End Sub

    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        Dim s As Screen
        Dim loadscreen As Integer = 0
        Try
            Dim sr As StreamReader = New StreamReader("ScreenPosition.cfg")
            loadscreen = sr.ReadLine
            sr.Close()
            s = screens(loadscreen)
        Catch ex As Exception

        End Try
        'Dim img As New Bitmap(s.Bounds.Width, s.Bounds.Height - 50)
        'Dim gr As Graphics = Graphics.FromImage(img)
        'Dim p As New Point(0, 50)
        'gr.CopyFromScreen(s.Bounds.Location, p, s.Bounds.Size)

        Dim img As New Bitmap(s.Bounds.Width, s.Bounds.Height - 90)
        Dim gr As Graphics = Graphics.FromImage(img)
        Dim p As New Point(0, 60)
        gr.CopyFromScreen(New Point(s.Bounds.Left, s.Bounds.Top + 60), p, New Size(s.Bounds.Width, s.Bounds.Height - 90))

        Using dialog As New SaveFileDialog()
            dialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|All Files|*.*"
            dialog.Title = "Save Image"
            dialog.FileName = "Cardiac Caliper " & DateTime.Now.ToString("yyyyMMdd_HHmmss")
            dialog.InitialDirectory = "C:\" ' Specify your initial directory here

            If dialog.ShowDialog() = DialogResult.OK Then
                Dim filePath As String = dialog.FileName

                ' Assuming you have an image object named "image" representing the image you want to save
                If img IsNot Nothing Then
                    img.Save(filePath)
                    MessageBox.Show("Image saved successfully.")
                Else
                    MessageBox.Show("No image to save.")
                End If
            End If
        End Using

    End Sub





    Private Sub Screen1_Click(sender As Object, e As EventArgs) Handles Screen1.Click
        Dim selectedIndex As Integer = 0
        If selectedIndex >= 0 AndAlso selectedIndex < screens.Length Then
            ' Change the form's screen to the selected screen
            Me.WindowState = FormWindowState.Normal
            Dim selectedScreen As Screen = screens(selectedIndex)
            Me.Location = selectedScreen.Bounds.Location
            Me.Size = selectedScreen.Bounds.Size
            Me.StartPosition = FormStartPosition.Manual ' Set form position manually
            Me.Location = selectedScreen.Bounds.Location ' Set the form's Screen property
        End If

        Screen1.Image = My.Resources._1Green2
        Screen2.Image = My.Resources._2Red2
        Screen3.Image = My.Resources._3Red2

        Dim sw As StreamWriter = New StreamWriter("ScreenPosition.cfg")
        sw.Write(selectedIndex, False)
        sw.Close()
    End Sub

    Private Sub Screen2_Click(sender As Object, e As EventArgs) Handles Screen2.Click
        Dim selectedIndex As Integer = 1
        If selectedIndex >= 0 AndAlso selectedIndex < screens.Length Then
            ' Change the form's screen to the selected screen
            Me.WindowState = FormWindowState.Normal
            Dim selectedScreen As Screen = screens(selectedIndex)
            Me.Location = selectedScreen.Bounds.Location
            Me.Size = selectedScreen.Bounds.Size
            Me.StartPosition = FormStartPosition.Manual ' Set form position manually
            Me.Location = selectedScreen.Bounds.Location ' Set the form's Screen property
        End If
        Screen1.Image = My.Resources._1Red2
        Screen2.Image = My.Resources._2Green2
        Screen3.Image = My.Resources._3Red2
        Dim sw As StreamWriter = New StreamWriter("ScreenPosition.cfg")
        sw.Write(selectedIndex, False)
        sw.Close()
    End Sub

    Private Sub Screen3_Click(sender As Object, e As EventArgs) Handles Screen3.Click
        Dim selectedIndex As Integer = 2
        If selectedIndex >= 0 AndAlso selectedIndex < screens.Length Then
            ' Change the form's screen to the selected screen
            Me.WindowState = FormWindowState.Normal
            Dim selectedScreen As Screen = screens(selectedIndex)
            Me.Location = selectedScreen.Bounds.Location
            Me.Size = selectedScreen.Bounds.Size
            Me.StartPosition = FormStartPosition.Manual ' Set form position manually
            Me.Location = selectedScreen.Bounds.Location ' Set the form's Screen property
        End If

        Screen1.Image = My.Resources._1Red2
        Screen2.Image = My.Resources._2Red2
        Screen3.Image = My.Resources._3Green2

        Dim sw As StreamWriter = New StreamWriter("ScreenPosition.cfg")
        sw.Write(selectedIndex, False)
        sw.Close()
    End Sub



    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        If listallsquares.Count >= 1 Then
            listallsquares(workingsquare).Copies = 0
        End If
        workingsquare = 0
        working = False
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        If listallsquares.Count >= 1 Then
            listallsquares(workingsquare).Copies = 0
        End If
        workingsquare = 1
        working = False
    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        If listallsquares.Count >= 1 Then
            listallsquares(workingsquare).Copies = 0
        End If
        workingsquare = 2
        working = False
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        If listallsquares.Count >= 1 Then
            listallsquares(workingsquare).Copies = 0
        End If
        workingsquare = 3
        working = False
    End Sub

    Private Sub ToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles ColorPicker.Click
        If listallsquares.Count >= 1 Then
            listallsquares(workingsquare).Copies = 0
        End If
        workingsquare = 4
        working = False
    End Sub



    Private Sub ToolStripMenuItem5_Click_1(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click
        ZoomDropDown.Text = "PDF Zoom: 100% "
        Zoom = 100
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem6_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem6.Click
        ZoomDropDown.Text = "PDF Zoom: 110% "
        Zoom = 110
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem7_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        ZoomDropDown.Text = "PDF Zoom: 125% "
        Zoom = 125
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem8_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        ZoomDropDown.Text = "PDF Zoom: 150% "
        Zoom = 150
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem9_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem9.Click
        ZoomDropDown.Text = "PDF Zoom: 175% "
        Zoom = 175
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem10_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem10.Click
        ZoomDropDown.Text = "PDF Zoom: 200% "
        Zoom = 200
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem11_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem11.Click
        ZoomDropDown.Text = "PDF Zoom: 250% "
        Zoom = 250
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem12_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        ZoomDropDown.Text = "PDF Zoom: 300% "
        Zoom = 300
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem13_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem13.Click
        ZoomDropDown.Text = "PDF Zoom: 400% "
        Zoom = 400
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub

    Private Sub ToolStripMenuItem14_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem14.Click
        ZoomDropDown.Text = "PDF Zoom: 500% "
        Zoom = 500
        modifiedratio = (ratio) / (Zoom / 100)
        Refresh()
    End Sub
End Class
