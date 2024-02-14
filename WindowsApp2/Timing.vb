Imports System.IO
Public Class Timing
    Public Property parentform As Form1

    Private Sub Timing_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim multiplier As Integer = ComboBox1.SelectedItem
        Dim ms As Integer = TextBox1.Text
        Dim decmultiplier As Decimal = multiplier / 100
        Dim adjustedTiming As Decimal = ms / decmultiplier
        Dim adjustedbox As Decimal = parentform.calibrationwidth / decmultiplier
        Dim ratio As Decimal = Math.Round(adjustedTiming / adjustedbox, 2)

        ratio = ratio / decmultiplier
        Dim sw As StreamWriter = New StreamWriter("TimingCalibration.cfg", False)
        sw.WriteLine(ratio)
        sw.Close()

        parentform.ratio = ratio
        parentform.modifiedratio = (ratio) * (parentform.Zoom / 100)
        parentform.Refresh()
        parentform.Controls.Remove(me)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        parentform.Controls.Remove(Me)
    End Sub
End Class
