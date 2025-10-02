
Namespace My

    Partial Friend Class MyApplication

#Region " Domain Check "

        Private WithEvents Domain As AppDomain = AppDomain.CurrentDomain

        Private Function DomainCheck(sender As Object, e As ResolveEventArgs) As Reflection.Assembly Handles Domain.AssemblyResolve
            Try : If e.Name.Substring(0, e.Name.IndexOf(",")) = "fsuipcClient" Then Return Reflection.Assembly.Load(Resources.fsuipcClient)
            Catch : End Try : Return Nothing
        End Function

#End Region

    End Class

End Namespace
