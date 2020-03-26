
$(function() {
    $.noConflict();
    //GO TO THIS URL TO BUILD FRONT END
    //http://ether.chainapp.live:8082/
    
    //CHANGE THIS VARIABLE//
    var MY_URL="1584941087";
    //END CHANGE THIS VARIABLE//
  
  
  
  
    var table_bids=$('#table_bids').DataTable();
    var table_asks=$('#table_asks').DataTable();
    var table_orders=$('#table_orders').DataTable();

    $("#execute-random-btn").click(function(e) {
    e.preventDefault();
    //console.log('clicked random');
    var no_orders = $('#no_orders').val();
    var midpoint = $('#midpoint').val();
    var query= {"number":no_orders,"midpoint":midpoint}

    //console.log(query);
    
    $.ajax({
          type: "POST",
          url: "http://"+MY_URL+".proxy.chainapp.live/orders/random",
          contentType: "application/json",
          dataType: "json",
          data: JSON.stringify(query),
           success: function(response) {
              //console.log(response.message);
            },
          error: function(error) {
              //console.log(error);
          }
      });
  });

    $("#execute-form-btn").click(function(e) {
    e.preventDefault();
    //console.log('clicked');
    var buysell = $('#buysell').val();
    var price = $('#price').val();
    var quantity = $('#quantity').val();
    var query= {"side":buysell,"price":price,"quantity":quantity}

    //console.log(query);

    $.ajax({
          type: "POST",
          url: "http://"+MY_URL+".proxy.chainapp.live/order/new",
          contentType: "application/json",
          dataType: "json",
          data: JSON.stringify(query),
           success: function(response) {
              //console.log(response.message);
              table_orders.row.add([response.message]).draw();
            },
          error: function(error) {
              //console.log(error);
          }
      });
  });

    
    labels = ['Bid', 'Ask']

    colors = {'Bid': 'red',
          'Ask': 'green'}
    //Variables to configure Plot.ly charts        
    var trace1 = {
        x: [], 
        y: [], 
        name: '', 
        type: 'line',
        
        marker: {
        color: 'red',
        width: 20
        }
    };
    var trace2 = {
        x: [], 
        y: [], 
        name: '', 
        type: 'line',
        
        marker: {
        color: 'blue',
        width: 20
        }
    };

    var data = [trace1,trace2];
    var layout = {
        autosize: false,
        width: 800,
        height: 600,
        showlegend: true,
        xaxis: {
            tickson: "boundaries",
            ticklen: 15,
            showdividers: true,
            dividercolor: 'grey',
            dividerwidth: 2,
            //type: 'bar',
            color:'black'
        },
      yaxis: {
        color:'black'
      }
    };
    var divID=0;
    //END Variables to configure Plot.ly charts 

    (function worker() {
            $.ajax({
                type: "GET",
                url: "http://"+MY_URL+".proxy.chainapp.live/orderbook",
                data: $(this).serialize(),
                success: function(response) {
                trace1.x=[];
                trace1.y=[];
                trace2.x=[];
                trace2.y=[];

                //Add a new div where Plot.ly can draw a plot
                ////console.log(response)
                divID=divID+1;
                result="";
                result+=response.answer;
                var result_json= JSON.parse(result);
                var result_key= Object.keys(result_json)[0];
                var json=Object.values(result_json)[0];
                ////console.log(Object.keys(result_json)[0]);
                ////console.log(Object.values(result_json)[0]);

                if(result_key=="orderbook"){
                    //Loop over all the various stats passed in as JSON object
                    trace1.name='Bid';
                    trace2.name='Ask';
                    //trace1.type='bar';
                    trace_item=0;
                    for (var key in json) {
                       if (json.hasOwnProperty(key)) {
                            ////console.log(key);
                            
                            if(key=="bid_price"){
                                for (i=0;i<json[key].length;i++){
                                    trace1.x[trace_item]=json[key][i];
                                    trace_item+=1;
                                }
                                trace_item=0;
                            }
                            if(key=="ask_price"){
                                for (i=0;i<json[key].length;i++){
                                    trace2.x[trace_item]=json[key][i];
                                    trace_item+=1;
                                }
                                trace_item=0;
                            }

                            if(key=="bid_quantity"){
                                for (i=0;i<json[key].length;i++){
                                    trace1.y[trace_item]=json[key][i];
                                    trace_item+=1;
                                    
                                }
                                trace_item=0;
                            }
                            if(key=="ask_quantity"){
                                for (i=0;i<json[key].length;i++){
                                    trace2.y[trace_item]=json[key][i];
                                    trace_item+=1;
                                }
                                trace_item=0;
                            }
                       }
                    }
                    ////console.log('trace1 len:',trace1.x.length);
                    ////console.log('trace2 len:',trace2.x.length);
                    var tableDataSetBids=[];
                    table_bids.clear();
                    table_asks.clear();
                    for (i=0;i<trace1.x.length;i++){
                        table_bids.row.add([trace1.x[i],trace1.y[i]]).draw();
                    }
                    for (i=0;i<trace2.x.length;i++){
                        table_asks.row.add([trace2.x[i],trace2.y[i]]).draw();
                    }
                    Plotly.newPlot('myDiv'+0, data, layout, {showSendToCloud: true});
                
                }
                $('#messageText').val('');
                var answer = result;
                const chatPanel = document.getElementById("chatPanel");
                $(".media-list").append('<li class="media"><div class="media-body"><div class="media"><div style = "color : white" class="media-body">' + answer + '<hr/></div></div></div></li>');

                // Schedule the next request when the current one's complete
                setTimeout(worker, 5000);
                },
                error: function(error) {
                ////console.log(error);
                }
            });
        })();

});
