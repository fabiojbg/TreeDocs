async function tree_fetch(url, data)
{
    try
    {
        if (!data.headers) data.headers = {};
        if( localStorage.getItem('authToken'))
            data.headers.Authorization = `Bearer ${localStorage.getItem('authToken')}`;
    
        const response = await fetch(`${API_BASE_URL}/${url}`, data);
        if( response.status == 401)
            window.location.href = 'login.html'; 
        const clonedResponse = response.clone();
        if( !clonedResponse.ok)
        {
            if( toastr )
            {
                const errData = await clonedResponse.json();
                if( errData._Message)
                {
                    toastr.error(errData._Message);
                }
            }
        }
        return response;
    }
    catch(err)
    {
        console.log("Network error: " + err);
        throw err;
    }
}
