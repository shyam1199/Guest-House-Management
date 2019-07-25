﻿Imports MySql.Data.MySqlClient

'this functions exactly like the online booking form, but this will be done by admin in case of offline bookings.

Public Class offline_bookingpage

    Dim mysqlcon As MySqlConnection
    Dim command As MySqlCommand
    Dim username As String
    Dim ss As Integer = 500              'cost of a single room of a student
    Dim ds As Integer = 700              'cost of a double room of a student
    Dim sp As Integer = 400             'cost of a single room of a proffessor or staff
    Dim dp As Integer = 500             'cost of a double room of a proffessor or staff
    Dim room As String
    Dim reader1 As MySqlDataReader
    Dim dt1 As Date
    Dim dt2 As Date
    Dim ts As Integer
    Dim duration As Integer
    Dim total As Integer=0
    Dim cleaning As Integer = 50
    Dim laundary As Integer = 20
    Public Sub rooms_available(ByVal startdate As Date, ByVal enddate As Date, ByRef single_rooms As Integer, ByRef double_rooms As Integer)
        mysqlcon = New MySqlConnection
        single_rooms = 0
        double_rooms = 0
        mysqlcon.ConnectionString = "server=localhost;userid=root;password=v7csWXtH;database=userinfo"         'connect to the database
        mysqlcon.Open()
        Dim query As String
        Dim command2 As MySqlCommand
        query = "Update userinfo.singleroom_table set Allotted = '0' "        'initially set Allotted=0 for all single rooms
        command2 = New MySqlCommand(query, mysqlcon)
        command2.ExecuteNonQuery()
        query = "Update userinfo.doubleroom_table set Allotted = '0' "         'initially set Allotted=0 for all double rooms
        command2 = New MySqlCommand(query, mysqlcon)
        command2.ExecuteNonQuery()
        query = "select * from userinfo.userinfo_table"
        command = New MySqlCommand(query, mysqlcon)
        reader1 = command.ExecuteReader

        Dim startdatetemp As Date                     'arrival date
        Dim enddatetemp As Date                       'departure date
        Dim mysqlcon2 As MySqlConnection
        mysqlcon2 = New MySqlConnection
        mysqlcon2.ConnectionString = "server=localhost;userid=root;password=v7csWXtH;database=userinfo"
        mysqlcon2.Open()
        While reader1.Read
            startdatetemp = CDate(reader1.GetString("Arrival_Date")).Date
            enddatetemp = CDate(reader1.GetString("end_date")).Date

            If ((startdate <= enddatetemp And startdate >= startdatetemp) Or (enddate >= startdatetemp And enddate <= enddatetemp) Or (startdate <= startdatetemp And enddate >= enddatetemp) Or (startdate >= startdatetemp And enddate <= enddatetemp)) Then
                Dim room_no As String = reader1.GetString("Room_No")

                If reader1.GetString("Room_type") = "Single" Then

                    query = "Update userinfo.singleroom_table set Allotted = '1' where Room_No= '" & room_no & "' "        'it becomes 1 when the room gets alotted
                    command2 = New MySqlCommand(query, mysqlcon2)
                    command2.ExecuteNonQuery()
                Else
                    query = "Update userinfo.doubleroom_table set Allotted = '1' where Room_No= '" & room_no & "' "         'it becomes 1 when the room gets alotted
                    command2 = New MySqlCommand(query, mysqlcon2)
                    command2.ExecuteNonQuery()
                End If

            End If
        End While
        mysqlcon2.Close()
        reader1.Close()
        mysqlcon.Close()
        'Catch ex As Exception
        'MessageBox.Show(ex.Message)
        ' Finally
        mysqlcon.Close()
        'End Try
        mysqlcon.Open()
        Dim query3 As String
        Dim command3 As MySqlCommand
        query3 = "select * from userinfo.singleroom_table where Allotted='0'"
        command3 = New MySqlCommand(query3, mysqlcon)
        Dim reader3 As MySqlDataReader
        reader3 = command3.ExecuteReader
        While (reader3.Read)
            single_rooms = single_rooms + 1         'counting the single rooms available
        End While
        command3.Dispose()
        query3 = "select * from userinfo.doubleroom_table where Allotted='0'"
        command3 = New MySqlCommand(query3, mysqlcon)
        reader3 = command3.ExecuteReader
        While (reader3.Read)
            double_rooms = double_rooms + 1         'counting the double rooms available
        End While
        reader3.Close()
        mysqlcon.Close()
    End Sub

    Private Sub book_btn_Click(sender As Object, e As EventArgs) Handles book_btn.Click
        Dim startdate As Date = datetxt.Text
        Dim enddate As Date = date2txt.Text
        Dim single_rooms, double_rooms As Integer
        rooms_available(startdate, enddate, single_rooms, double_rooms)
        mysqlcon = New MySqlConnection
        mysqlcon.ConnectionString = "server=localhost;userid=root;password=v7csWXtH;database=userinfo"
        Dim reader As MySqlDataReader
        If contacttxt.Text = "" Or addresstxt.Text = "" Or nametxt.Text = "" Or purposetxt.Text = "" Or datetxt.Text = "" Or roomtype.Text = "" Or cleaningtxt.Text = "" Or Laundarytxt.Text = "" Then
            MsgBox("Enter your correct basic details")    'don't leave any field empty
        Else

            If IsAlpha(nametxt.Text) = False Then
                MsgBox("Enter valid name")     'names shouldn't usually contain numbers, should they?
            Else
                dt1 = DateTimePicker1.Text
                dt2 = datetxt.Text
                ts = dt2.Subtract(dt1).Days
                duration = ts + 1
                If duration <= 0 Then
                    MsgBox("Please Select correct arrival date")
                Else
                    If duration > 10 Then
                        MsgBox("Prebooking limit exceeded")        'you're not pre booking 10 days earlier
                    Else
                        dt1 = datetxt.Text
                        dt2 = date2txt.Text
                        ts = dt2.Subtract(dt1).Days
                        duration = ts + 1
                        If duration <= 0 Then
                            MsgBox("Departure date should be greater than arrival date")    'unless you have discovered time travel, in which case, contact the Physics dept maybe
                        Else
                            If duration > 5 Then
                                MsgBox("Cannot stay for more than 5 days")
                            Else

                                If IsNum(contacttxt.Text) = False Then
                                    MsgBox("Enter 10 digit mobile no")        'all of them are 10 digits, aren't they?
                                Else


                                    If Val(Laundarytxt.Text) < 0 Or Val(Laundarytxt.Text) > duration Then
                                        MsgBox("Enter correct laundary days")        'you're not doing laundry for more days than you're staying for
                                    Else
                                        If Val(cleaningtxt.Text) < 0 Or Val(cleaningtxt.Text) > duration Then
                                            MsgBox("Enter correct cleaning days")     'again, you don't need to pay to clean for others
                                        Else
                                            If roomtype.Text = "Single" And (category.Text = "student" Or category.Text = "None") Then
                                                total = ss * (duration) + cleaning * Val(cleaningtxt.Text) + laundary * Val(Laundarytxt.Text)
                                            End If
                                            If roomtype.Text = "Single" And (category.Text = "proffessor" Or category.Text = "staff") Then
                                                total = sp * (duration) + cleaning * Val(cleaningtxt.Text) + laundary * Val(Laundarytxt.Text)
                                            End If
                                            If roomtype.Text = "Double" And (category.Text = "student" Or category.Text = "None") Then
                                                total = ds * (duration) + cleaning * Val(cleaningtxt.Text) + laundary * Val(Laundarytxt.Text)
                                            End If
                                            If roomtype.Text = "Double" And (category.Text = "proffessor" Or category.Text = "staff") Then
                                                total = dp * (duration) + cleaning * Val(cleaningtxt.Text) + laundary * Val(Laundarytxt.Text)
                                            End If
                                            Dim response1 = MsgBox("Total amount = " & total & "     Do you want to pay?", vbYesNo, "Amount Box")
                                            If response1 = vbYes Then
                                                If roomtype.Text = "Single" Then
                                                    If single_rooms = 0 Then
                                                        MsgBox("No vacant single rooms")
                                                    Else
                                                        Try
                                                            mysqlcon.Open()
                                                            Dim query As String
                                                            query = "select * from userinfo.singleroom_table where Allotted='" & 0 & "'"  'choose an empty room
                                                            command = New MySqlCommand(query, mysqlcon)
                                                            reader = command.ExecuteReader
                                                            Dim count = 0
                                                            While reader.Read
                                                                count = count + 1
                                                                If count = 1 Then
                                                                    room = reader.GetString("Room_No")           'and give it to the user
                                                                End If
                                                            End While
                                                            mysqlcon.Close()
                                                        Catch ex As Exception
                                                            MessageBox.Show(ex.Message)
                                                        Finally
                                                            mysqlcon.Dispose()
                                                        End Try

                                                        Try
                                                            mysqlcon.Open()
                                                            Dim query As String
                                                            Dim startdatetemp1 As Date = datetxt.Text
                                                            Dim enddatetemp1 As Date = date2txt.Text
                                                            query = "insert into userinfo.userinfo_table (Room_No,Webmail,Name,Address,Contact_no,Purpose_of_visit,Arrival_Date,end_date,Laundary,Cleaning,Room_Type) values ('" & room & "','" & webmailtxt.Text & "','" & nametxt.Text & "','" & addresstxt.Text & "','" & contacttxt.Text & "','" & purposetxt.Text & "','" & startdatetemp1.Date & "','" & enddatetemp1.Date & "','" & Laundarytxt.Text & "','" & cleaningtxt.Text & "','" & roomtype.Text & "')"
                                                            command = New MySqlCommand(query, mysqlcon)
                                                            reader = command.ExecuteReader
                                                            mysqlcon.Close()
                                                        Catch ex As Exception
                                                            MessageBox.Show(ex.Message)
                                                        Finally
                                                            mysqlcon.Dispose()
                                                        End Try
                                                        registereduser_details.Show()
                                                        Me.Close()
                                                    End If

                                                ElseIf roomtype.Text = "Double" Then
                                                    If double_rooms = 0 Then
                                                        MsgBox("No vacant double rooms")
                                                    Else
                                                        Try
                                                            mysqlcon.Open()
                                                            Dim query As String
                                                            query = "select * from userinfo.doubleroom_table where Allotted='" & 0 & "'"
                                                            command = New MySqlCommand(query, mysqlcon)
                                                            reader = command.ExecuteReader
                                                            Dim count = 0
                                                            While reader.Read
                                                                count = count + 1
                                                                If count = 1 Then
                                                                    room = reader.GetString("Room_No")
                                                                End If
                                                            End While
                                                            mysqlcon.Close()
                                                        Catch ex As Exception
                                                            MessageBox.Show(ex.Message)
                                                        Finally
                                                            mysqlcon.Dispose()
                                                        End Try

                                                        Try
                                                            mysqlcon.Open()
                                                            Dim query As String
                                                            Dim startdatetemp1 As Date = datetxt.Text
                                                            Dim enddatetemp1 As Date = date2txt.Text
                                                            query = "insert into userinfo.userinfo_table (Room_No,Webmail,Name,Address,Contact_no,Purpose_of_visit,Arrival_Date,end_date,Laundary,Cleaning,Room_Type) values ('" & room & "','" & webmailtxt.Text & "','" & nametxt.Text & "','" & addresstxt.Text & "','" & contacttxt.Text & "','" & purposetxt.Text & "','" & startdatetemp1.Date & "','" & enddatetemp1.Date & "','" & Laundarytxt.Text & "','" & cleaningtxt.Text & "','" & roomtype.Text & "')"
                                                            command = New MySqlCommand(query, mysqlcon)
                                                            reader = command.ExecuteReader
                                                            mysqlcon.Close()
                                                        Catch ex As Exception
                                                            MessageBox.Show(ex.Message)
                                                        Finally
                                                            mysqlcon.Dispose()
                                                        End Try
                                                        registereduser_details.Show()
                                                        Me.Close()
                                                    End If
                                                End If
                                            Else
                                                Dim response2 = MsgBox("want to stay in this page", vbYesNo)   'if you exit
                                                If response2 = vbNo Then
                                                    MsgBox("Booking cancelled")
                                                    mainpage.Show()
                                                    Me.Close()
                                                End If
                                            End If

                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim startdate As Date = datetxt.Text
        Dim enddate As Date = date2txt.Text
        Dim single_rooms, double_rooms As Integer
        rooms_available(startdate, enddate, single_rooms, double_rooms)
        MsgBox("Single rooms = " & single_rooms & " and " & "Double rooms = " & double_rooms)
    End Sub

    Private Sub Back_Click(sender As Object, e As EventArgs) Handles Back.Click
        registereduser_details.Show()
        Me.Close()
    End Sub
    Public Function IsAlpha(ByVal strInputText As String) As Boolean
        Dim IsAlpha1 As Boolean = False
        If System.Text.RegularExpressions.Regex.IsMatch(strInputText, "^[a-zA-Z]+[' ']*[a-zA-Z]*[' ']*[a-zA-Z]*$") Then
            IsAlpha1 = True
        Else
            IsAlpha1 = False
        End If
        Return IsAlpha1
    End Function

    Public Function IsNum(ByVal strInputText As String) As Boolean
        Dim IsAlpha As Boolean = False
        If System.Text.RegularExpressions.Regex.IsMatch(strInputText, "^[1-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$") Then
            IsAlpha = True
        Else
            IsAlpha = False
        End If
        Return IsAlpha
    End Function

    Private Sub offline_bookingpage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        DateTimePicker1.Hide()
        registereduser_details.Button1.Show()
        registereduser_details.singletxt.Hide()
        registereduser_details.doubletxt.Hide()
        registereduser_details.Label15.Hide()
        registereduser_details.Label16.Hide()
    End Sub
End Class