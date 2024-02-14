Public Class AllSquares
    Public Property Square As Rectangle
    Public Property Pen As Pen
    Public Property ID As Integer
    Public Property Notation As String
    Public Property Copies As Integer = 0

    Public Shared Sub LoadSquares()
        Dim y As List(Of Color) = New List(Of Color)
        Dim i As Color = Color.Red
        y.Add(i)
        i = Color.Blue
        y.Add(i)
        i = Color.LimeGreen
        y.Add(i)
        i = Color.Purple
        y.Add(i)
        i = Color.Yellow
        y.Add(i)

        For x = 0 To y.Count - 1
            Dim R As New AllSquares
            Dim pen1 As New Pen(y(x), 1)
            R.pen = pen1
            R.ID = x
            Dim p As New Point(0, 0)
            Dim s As New Size(0, 0)
            R.square = New Rectangle(p, s)

            Form1.listallsquares.Add(R)
        Next
    End Sub
End Class
