<!DOCTYPE html>

<?php include_once('get_manual.php'); ?>

<html>


<head>
    <link rel="stylesheet" type="text/css" href="style.css">
</head>

<body>

    <div id="manual_form">
        <image id="title_image" src="mooc_text_logo.png"></image>
        <h2>Mission Control Manual Archive</h2>
        <br>
        <div id="error_space"><?php echo($error); ?></div>
        <br>
        <form action="/" method="post">
            <p><label for="manual_code">Enter your secret manual code here:</label></p>
            <input type="text" name="manual_code">
            <br>
            <br>
            <input id="button_image" type="image" src="mooc_confirm.png" alt="Submit">
        </form>
    </div>


</body>

</html>
