<?php
    
    $error = "";

    if (!empty($_POST['manual_code'])) {
        $code = $_POST['manual_code'];
        $filename = "Mission_Manual_" . $code . ".html";

        if(file_exists($filename)) {
            $error = "";
            header("Location: /" . $filename);
        } else {
            $error = "CAN'T FIND THIS MANUAL. PLEASE TRY AGAIN.";
        }
    }
?>