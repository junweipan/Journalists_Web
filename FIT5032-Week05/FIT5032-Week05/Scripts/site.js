$(document).ready(function () {
    // Setup - add a text input to each footer cell
    $('#example2 tfoot th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" placeholder="Search ' + title + '" />');
    });

    // DataTable
    var table = $('#example2').DataTable();

    // Apply the search
    table.columns().every(function () {
        var that = this;

        $('input', this.footer()).on('keyup change', function () {
            if (that.search() !== this.value) {
                that
                    .search(this.value)
                    .draw();
            }
        });
    });
});
$(document).ready(function () {
    $('#example').DataTable();
});
$(document).ready(function () 
{
  $('#data').dataTable({
      "sDom": '<"nav"lf>t<"nav"i>'
  });
});

$('#date-input').datepicker({
    format: 'yyyy-mm-dd'
});



