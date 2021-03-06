﻿
Namespace MenuProject

    Public Class mainForm : Inherits Form
        Dim closeButtonPosition As Rectangle
        Dim HoveredTabIndex As Integer
        Dim CloseImageDynamic As Bitmap = Images.Default
        Dim isMouseLeftDown As Boolean = False

        Private Sub Form1_Load(ByVal sender As Object, _
            ByVal e As System.EventArgs) Handles MyBase.Load

            Me.TabControl1.Padding = New System.Drawing.Point(25, 0)
            menuStripFormation.buildMenuStrip(Me)

        End Sub


        Private Sub MainForm_MdiChildActivate(sender As Object, e As EventArgs) Handles MyBase.MdiChildActivate
            ' If there are no child form, hide tabControl
            If Me.ActiveMdiChild Is Nothing Then
                TabControl1.Visible = False
            Else
                TabControl1.Visible = True

                ' If child form is new
                If Me.ActiveMdiChild.Tag Is Nothing Then
                    'create tab page 
                    Dim tp As New TabPage()
                    tp.Text = Me.ActiveMdiChild.Text
                    tp.Tag = Me.ActiveMdiChild
                    TabControl1.TabPages.Add(tp)

                    'connect child form
                    tp.Parent = TabControl1
                    TabControl1.SelectedTab = tp
                    Me.ActiveMdiChild.Tag = tp

                End If
            End If
        End Sub


        'focuses close button on selected tab
        Private Sub focusCloseButton()
            Dim rect As Rectangle = TabControl1.GetTabRect(HoveredTabIndex)
            closeButtonPosition = New Rectangle(rect.Right - 15, rect.Top + 4, CloseImageDynamic.Width, CloseImageDynamic.Height)
        End Sub

        Public NotInheritable Class Images
            Public Shared [Default] As New Bitmap(My.Resources._default)
            Public Shared Hover As New Bitmap(My.Resources.hover)
            Public Shared Press As New Bitmap(My.Resources.press)
        End Class

#Region "tabControl1 Events"

        Private Sub tabControl1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControl1.DrawItem
            Dim CloseImageTarget As Bitmap = CloseImageDynamic

            If e.Index <> HoveredTabIndex Then
                CloseImageTarget = Images.[Default]
            End If

            e.Graphics.DrawString(
                Me.TabControl1.TabPages(e.Index).Text,
                e.Font,
                Brushes.Black,
                e.Bounds.Left + 5,
                e.Bounds.Top + 4)
             
            e.Graphics.DrawImage(
                CloseImageTarget,
                e.Bounds.Right - CloseImageDynamic.Width - 3,
                e.Bounds.Top + 3,
                CloseImageDynamic.Width,
                CloseImageDynamic.Height)

        End Sub

        Private Sub tabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
            If (TabControl1.SelectedTab IsNot Nothing) AndAlso (TabControl1.SelectedTab.Tag IsNot Nothing) Then
                TryCast(TabControl1.SelectedTab.Tag, Form).[Select]()
            End If

            If TabControl1.SelectedIndex = -1 Then
                Return
            End If
        End Sub

        Private Sub tabControl1_MouseClick(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseClick

            If closeButtonPosition.Contains(e.Location) AndAlso MessageBox.Show("Would you like to Close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                'dispose the Form
                TryCast(Me.TabControl1.TabPages(TabControl1.SelectedIndex).Tag, Form).Dispose()

                'Remove the Tab
                Me.TabControl1.TabPages.RemoveAt(TabControl1.SelectedIndex)

                'after closing a tab, bring focus to the last tab
                'The if statement prevents error when there is only a single tab left
                If TabControl1.TabCount = 0 Then
                    Return
                End If
                TabControl1.SelectedTab = Me.TabControl1.TabPages(TabControl1.TabCount - 1)
                
            End If
        End Sub

        Private Sub tabControl1_MouseLeave(sender As Object, e As EventArgs) Handles TabControl1.MouseLeave
            CloseImageDynamic = Images.[Default]
            TabControl1.Invalidate()
        End Sub

        Private Sub tabControl1_MouseDown(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseDown
            isMouseLeftDown = True

            If closeButtonPosition.Contains(e.Location) Then
                CloseImageDynamic = Images.Press
            Else
                CloseImageDynamic = Images.Default
            End If

            TabControl1.Invalidate()
        End Sub

        Private Sub tabControl1_MouseMove(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseMove

            'Sets tab index for Close button to reference.
            For i As Integer = 0 To TabControl1.TabCount - 1
                If TabControl1.GetTabRect(i).Contains(e.Location) Then
                    HoveredTabIndex = i
                    focusCloseButton()
                    Exit For
                End If
            Next


            'keep the press down image when moving
            If isMouseLeftDown Then
                Return
            End If

            'redraw to hover when moving onto the button
            If closeButtonPosition.Contains(e.Location) Then
                If CloseImageDynamic IsNot Images.Hover Then
                    CloseImageDynamic = Images.Hover
                Else
                    Return
                End If

                'redraw to default when moving off the button
            ElseIf CloseImageDynamic IsNot Images.[Default] Then
                CloseImageDynamic = Images.[Default]
            Else
                Return
            End If

            TabControl1.Invalidate()
        End Sub

        Private Sub tabControl1_MouseUp(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseUp
            isMouseLeftDown = False
        End Sub
#End Region

    End Class

End Namespace

