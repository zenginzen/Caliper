Public Class Notation
    Public Property Measurement As AllSquares
    Public Property ParentForm As Form1



    Private Sub Notation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BackColor = Color.FromArgb(220, 5, 77)
        ColorLabel.Text = "Notation for " & Measurement.Pen.Color.ToString & " Measurement"
        RichTextBox1.Text = Measurement.Notation
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.Controls.Remove(Me)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Measurement.Notation = RichTextBox1.Text
        Me.Visible = False
        ParentForm.workingsquare = "99"
        ParentForm.Refresh()

        ParentForm.workingsquare = Measurement.ID
        Form1.Controls.Remove(Me)

    End Sub
End Class
