// Playlist next tune pointer 
var playlistIndex = -2;

// controller parm contains the encoded MVC action URL- cant use razor inside pure javascript file .
function initPlaylist(controller)
{
    playlistIndex = -1;
    loadNextTune(controller);
}

function loadNextTune(controller)
{
    if (playlistIndex > -2)
    {
        playlistIndex++;

        var totalTunes = $('#PlaylistDiv tr').length;

        // Load next tune in playlist or reset if complete
        if (playlistIndex <= totalTunes - 1) {
            // Next tune
            $.get(controller, { position: playlistIndex }, function (data) {
                $('#AudioControlPanel').html(data);
            })
        }
        else {
            // End of playlist, reset 
            playlistIndex = -2;
        }

        
    }
}